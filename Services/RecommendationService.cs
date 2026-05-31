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

            // 1. Descarte crítico por salud (Alergias y Restricciones Dietéticas)
            // Evaluamos intolerancias al Gluten, Lactosa y dieta Vegana de forma proactiva.
            bool userHasGlutenSensitivity = 
                preferencia.Alergias?.Contains("Gluten", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.RestriccionesDieteticas?.Contains("Gluten", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.Alergias?.Contains("Sin Gluten", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.RestriccionesDieteticas?.Contains("Sin Gluten", StringComparison.OrdinalIgnoreCase) == true;

            bool userHasLactoseSensitivity = 
                preferencia.Alergias?.Contains("Lactosa", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.RestriccionesDieteticas?.Contains("Lactosa", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.Alergias?.Contains("Sin Lactosa", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.RestriccionesDieteticas?.Contains("Sin Lactosa", StringComparison.OrdinalIgnoreCase) == true;

            bool userIsVegan = 
                preferencia.Alergias?.Contains("Vegano", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.RestriccionesDieteticas?.Contains("Vegano", StringComparison.OrdinalIgnoreCase) == true;

            // Analizamos el plato para ver si contiene Gluten
            bool platoContainsGluten = 
                plato.NombrePlato.Contains("Pan", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Pasta", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("crutones", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("brioche", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("fettuccine", StringComparison.OrdinalIgnoreCase) ||
                plato.Ingredientes.Any(i => i.NombreIngr.Contains("Gluten", StringComparison.OrdinalIgnoreCase) || 
                                             i.NombreIngr.Contains("Pan", StringComparison.OrdinalIgnoreCase) ||
                                             i.NombreIngr.Contains("Crutones", StringComparison.OrdinalIgnoreCase) ||
                                             i.NombreIngr.Contains("Fettuccine", StringComparison.OrdinalIgnoreCase));

            // Analizamos el plato para ver si contiene Lactosa
            bool platoContainsLactose = 
                plato.Descripcion.Contains("queso", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("crema", StringComparison.OrdinalIgnoreCase) ||
                plato.Ingredientes.Any(i => i.NombreIngr.Contains("Lactosa", StringComparison.OrdinalIgnoreCase) || 
                                             i.NombreIngr.Contains("Queso", StringComparison.OrdinalIgnoreCase) ||
                                             i.NombreIngr.Contains("Crema", StringComparison.OrdinalIgnoreCase));

            // Analizamos el plato para ver si contiene productos de origen animal
            bool platoContainsAnimalProducts = 
                platoContainsLactose ||
                plato.NombrePlato.Contains("Tacos", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Hamburguesa", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Pollo", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Cerdo", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Res", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Salmón", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("pollo", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("cerdo", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("salmón", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("res", StringComparison.OrdinalIgnoreCase) ||
                plato.Ingredientes.Any(i => i.NombreIngr.Contains("Pollo", StringComparison.OrdinalIgnoreCase) ||
                                             i.NombreIngr.Contains("Cerdo", StringComparison.OrdinalIgnoreCase) ||
                                             i.NombreIngr.Contains("Carne", StringComparison.OrdinalIgnoreCase) ||
                                             i.NombreIngr.Contains("Salmón", StringComparison.OrdinalIgnoreCase) ||
                                             i.NombreIngr.Contains("Res", StringComparison.OrdinalIgnoreCase));

            // Evaluamos descartes drásticos de salud
            if (userHasGlutenSensitivity && platoContainsGluten) return 0.0m;
            if (userHasLactoseSensitivity && platoContainsLactose) return 0.0m;
            if (userIsVegan && platoContainsAnimalProducts) return 0.0m;

            // Filtro general por coincidencias de texto de alergias específicas en el plato o sus ingredientes
            if (!string.IsNullOrEmpty(preferencia.Alergias))
            {
                var allergies = preferencia.Alergias.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var allergy in allergies)
                {
                    if (plato.NombrePlato.Contains(allergy, StringComparison.OrdinalIgnoreCase) ||
                        plato.Descripcion.Contains(allergy, StringComparison.OrdinalIgnoreCase) ||
                        plato.Ingredientes.Any(i => i.NombreIngr.Contains(allergy, StringComparison.OrdinalIgnoreCase)))
                    {
                        return 0.0m;
                    }
                }
            }

            // Filtro general por restricciones dietéticas adicionales
            if (!string.IsNullOrEmpty(preferencia.RestriccionesDieteticas))
            {
                var restrictions = preferencia.RestriccionesDieteticas.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var restriction in restrictions)
                {
                    if (plato.NombrePlato.Contains(restriction, StringComparison.OrdinalIgnoreCase) ||
                        plato.Descripcion.Contains(restriction, StringComparison.OrdinalIgnoreCase) ||
                        plato.Ingredientes.Any(i => i.NombreIngr.Contains(restriction, StringComparison.OrdinalIgnoreCase)))
                    {
                        return 0.0m;
                    }
                }
            }

            // 2. Coincidencia de tipo de cocina (Si coincide se mantiene alto; si no, penalización del 30%)
            bool matchesCuisine = false;
            string favCuisine = preferencia.TipoCocinaFavorita;
            
            if (!string.IsNullOrEmpty(favCuisine))
            {
                if (favCuisine.Contains("Italiana", StringComparison.OrdinalIgnoreCase) && 
                    (plato.NombrePlato.Contains("Pasta", StringComparison.OrdinalIgnoreCase) || 
                     plato.NombrePlato.Contains("César", StringComparison.OrdinalIgnoreCase) || 
                     plato.NombrePlato.Contains("Pizza", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("Alfredo", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("parmesano", StringComparison.OrdinalIgnoreCase) ||
                     plato.Descripcion.Contains("italiana", StringComparison.OrdinalIgnoreCase) ||
                     plato.Descripcion.Contains("margherita", StringComparison.OrdinalIgnoreCase)))
                {
                    matchesCuisine = true;
                }
                else if (favCuisine.Contains("Mexicana", StringComparison.OrdinalIgnoreCase) && 
                    (plato.NombrePlato.Contains("Tacos", StringComparison.OrdinalIgnoreCase) || 
                     plato.NombrePlato.Contains("Burrito", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("cerdo marinado", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("tortillas", StringComparison.OrdinalIgnoreCase) ||
                     plato.Descripcion.Contains("mexicana", StringComparison.OrdinalIgnoreCase)))
                {
                    matchesCuisine = true;
                }
                else if (favCuisine.Contains("Asiática", StringComparison.OrdinalIgnoreCase) && 
                    (plato.NombrePlato.Contains("Sushi", StringComparison.OrdinalIgnoreCase) || 
                     plato.NombrePlato.Contains("Ramen", StringComparison.OrdinalIgnoreCase) || 
                     plato.NombrePlato.Contains("Thai", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("nori", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("sushi", StringComparison.OrdinalIgnoreCase) ||
                     plato.Descripcion.Contains("asiático", StringComparison.OrdinalIgnoreCase)))
                {
                    matchesCuisine = true;
                }
            }

            if (!matchesCuisine)
            {
                matchScore -= 0.30m; // Penalización del 30% si no coincide con su cocina favorita
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
