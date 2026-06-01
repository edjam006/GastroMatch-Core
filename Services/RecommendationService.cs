using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GastroMatch_Core.Data;
using GastroMatch_Core.Models;
using GastroMatch_Core.Controllers;

namespace GastroMatch_Core.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly AppDbContext _context;

        public RecommendationService(AppDbContext context)
        {
            _context = context;
        }

        public decimal CalculateMatch(PreferenciaUsuario preferencia, Plato plato)
        {
            if (preferencia == null || plato == null)
                return 0.0m;

            decimal matchScore = 1.0m; // Match base de 100%

            // 1. Filtro de Salud e Ingredientes Genérico (Dinámico)
            var healthTokens = new List<string>();

            if (!string.IsNullOrEmpty(preferencia.Alergias))
            {
                var tokens = preferencia.Alergias.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                healthTokens.AddRange(tokens);
            }

            if (!string.IsNullOrEmpty(preferencia.RestriccionesDieteticas))
            {
                var tokens = preferencia.RestriccionesDieteticas.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                healthTokens.AddRange(tokens);
            }

            // Eliminar duplicados para eficiencia
            healthTokens = healthTokens.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var token in healthTokens)
            {
                // Validación crítica de seguridad: comparación de subcadena insensible a mayúsculas/minúsculas
                if (plato.NombrePlato.Contains(token, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(plato.Descripcion) && plato.Descripcion.Contains(token, StringComparison.OrdinalIgnoreCase)) ||
                    (plato.Ingredientes != null && plato.Ingredientes.Any(i => i.NombreIngr.Contains(token, StringComparison.OrdinalIgnoreCase))))
                {
                    return 0.0m; // Early exit de seguridad sanitaria
                }
            }

            // 2. Filtro de Cocina Dinámico
            string? favCuisine = preferencia.TipoCocinaFavorita;
            string? restCuisine = plato.Restaurante?.TipoCocina;

            if (!string.IsNullOrEmpty(restCuisine))
            {
                bool matchesCuisine = !string.IsNullOrEmpty(favCuisine) && 
                                     favCuisine.Contains(restCuisine, StringComparison.OrdinalIgnoreCase);

                if (!matchesCuisine)
                {
                    matchScore -= 0.30m; // Penalización suave por cocina del -30%
                }
            }

            // Aseguramos que la afinidad nunca sea negativa
            return Math.Max(0.0m, matchScore);
        }

        private static double CalcularDistanciaHaversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0; // Radio de la Tierra en kilómetros

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2.0) * Math.Sin(dLat / 2.0) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2.0) * Math.Sin(dLon / 2.0);

            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

            return R * c;
        }

        private static double ToRadians(double angle)
        {
            return (Math.PI / 180.0) * angle;
        }

        public List<PlatoRecommendationViewModel> GetTopPicks(PreferenciaUsuario preferencias)
        {
            if (preferencias == null)
                return new List<PlatoRecommendationViewModel>();

            // Obtener la Latitud y Longitud del usuario autenticado
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == preferencias.UsuarioId);
            double userLat = (double)(usuario?.Latitud ?? -0.182778m);
            double userLon = (double)(usuario?.Longitud ?? -78.484167m);

            // Obtener platos de la base de datos con sus relaciones correspondientes
            var dbPlates = _context.Platos
                .Include(p => p.Restaurante)
                .Include(p => p.Ingredientes)
                .ToList();

            // Pipeline unificado: calcular score, distancia y porcentaje en un único bucle
            var scoredPlates = new List<(Plato Plato, decimal Score, int MatchPercentage, double DistanceKm)>();
            foreach (var plato in dbPlates)
            {
                decimal matchScore = CalculateMatch(preferencias, plato);
                int matchPercentage = (int)(matchScore * 100m);
                
                double restLat = (double)(plato.Restaurante?.Latitud ?? -0.182778m);
                double restLon = (double)(plato.Restaurante?.Longitud ?? -78.484167m);
                double distanceKm = CalcularDistanciaHaversine(userLat, userLon, restLat, restLon);
                
                scoredPlates.Add((plato, matchScore, matchPercentage, distanceKm));
            }

            // Filtrar, ordenar y proyectar al ViewModel en un flujo LINQ continuo
            var recommendations = scoredPlates
                .Where(sp => sp.Plato.Precio <= preferencias.RangoPrecioMax && sp.Score > 0m)
                .OrderByDescending(sp => sp.Score)
                .Take(3)
                .Select(sp => new Controllers.PlatoRecommendationViewModel
                {
                    Plato = sp.Plato,
                    MatchPercentage = sp.MatchPercentage,
                    DistanceKm = sp.DistanceKm
                })
                .ToList();

            return recommendations;
        }
    }
}
