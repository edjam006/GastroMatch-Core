using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GastroMatch_Core.Data;
using GastroMatch_Core.Models;
using GastroMatch_Core.Controllers;
using GastroMatch_Core.Services.Filters;

namespace GastroMatch_Core.Services
{
    /// <summary>
    /// Fachada principal del motor de recomendaciones de GastroMatch.
    /// 
    /// Patrón Facade: esta clase actúa como punto de entrada de alto nivel que coordina
    /// múltiples subsistemas (cálculo de distancia, fábrica de filtros, pipeline de evaluación)
    /// detrás de una interfaz simplificada (IRecommendationService).
    /// 
    /// Principio DIP: depende exclusivamente de abstracciones (IDistanceCalculator,
    /// IRecommendationFilterFactory), nunca de implementaciones concretas.
    /// 
    /// Principio SRP: su única responsabilidad es orquestar el flujo de recomendación,
    /// delegando cada regla de negocio a su filtro correspondiente.
    /// </summary>
    public class RecommendationService : IRecommendationService
    {
        private readonly AppDbContext _context;
        private readonly IDistanceCalculator _distanceCalculator;
        private readonly IRecommendationFilterFactory _filterFactory;

        // Coordenadas por defecto: centro de Quito, Ecuador (fallback geodésico)
        private const decimal DefaultLatitude = -0.182778m;
        private const decimal DefaultLongitude = -78.484167m;

        public RecommendationService(
            AppDbContext context,
            IDistanceCalculator distanceCalculator,
            IRecommendationFilterFactory filterFactory)
        {
            _context = context;
            _distanceCalculator = distanceCalculator;
            _filterFactory = filterFactory;
        }

        /// <summary>
        /// Calcula el porcentaje de afinidad entre las preferencias del usuario y un plato específico.
        /// Delega internamente al pipeline de filtros Strategy para mantener consistencia.
        /// Mantiene la firma original por retrocompatibilidad con IRecommendationService.
        /// </summary>
        public decimal CalculateMatch(PreferenciaUsuario preferencia, Plato plato)
        {
            if (preferencia == null || plato == null)
                return 0.0m;

            decimal score = 1.0m; // Match base de 100%

            // Ejecutar el pipeline de filtros Strategy sobre el plato
            var filters = _filterFactory.CreateFilters(preferencia);

            foreach (var filter in filters)
            {
                var result = filter.Apply(plato, preferencia, score);

                if (result.IsExcluded)
                    return 0.0m; // Early exit propagado desde el filtro

                score = result.Score;
            }

            return score;
        }

        /// <summary>
        /// Obtiene las 3 mejores recomendaciones de platos para el usuario.
        /// 
        /// Flujo de la Fachada (Facade Pattern):
        /// 1. Obtiene coordenadas del usuario (fallback: centro de Quito).
        /// 2. Carga platos con relaciones (Restaurante, Ingredientes) desde la BD.
        /// 3. Invoca la Factory para ensamblar el pipeline de filtros según el perfil.
        /// 4. Ejecuta el pipeline Strategy sobre cada plato candidato.
        /// 5. Calcula la distancia Haversine vía el servicio inyectado (DIP).
        /// 6. Aplica el pipeline LINQ final: filtrar, ordenar y proyectar al ViewModel.
        /// </summary>
        public List<PlatoRecommendationViewModel> GetTopPicks(PreferenciaUsuario preferencias)
        {
            if (preferencias == null)
                return new List<PlatoRecommendationViewModel>();

            // ── Paso 1: Obtener coordenadas del usuario autenticado ──
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == preferencias.UsuarioId);
            double userLat = (double)(usuario?.Latitud ?? DefaultLatitude);
            double userLon = (double)(usuario?.Longitud ?? DefaultLongitude);

            // ── Paso 2: Cargar platos con sus relaciones desde la base de datos ──
            var dbPlates = _context.Platos
                .Include(p => p.Restaurante)
                .Include(p => p.Ingredientes)
                .ToList();

            // ── Paso 3: Ensamblar el pipeline de filtros vía Factory Method ──
            var filters = _filterFactory.CreateFilters(preferencias);

            // ── Paso 4: Evaluar cada plato a través del pipeline de estrategias ──
            var scoredPlates = new List<(Plato Plato, decimal Score, int MatchPercentage, double DistanceKm)>();

            foreach (var plato in dbPlates)
            {
                // Ejecutar pipeline de filtros Strategy
                decimal matchScore = 1.0m; // Match base de 100%
                bool excluded = false;

                foreach (var filter in filters)
                {
                    var result = filter.Apply(plato, preferencias, matchScore);

                    if (result.IsExcluded)
                    {
                        excluded = true;
                        break; // Early exit del pipeline
                    }

                    matchScore = result.Score;
                }

                if (excluded || matchScore <= 0m)
                    continue; // Plato excluido — no agregar al pipeline LINQ

                int matchPercentage = (int)(matchScore * 100m);

                // ── Paso 5: Calcular distancia geográfica vía IDistanceCalculator (DIP) ──
                double restLat = (double)(plato.Restaurante?.Latitud ?? DefaultLatitude);
                double restLon = (double)(plato.Restaurante?.Longitud ?? DefaultLongitude);
                double distanceKm = _distanceCalculator.Calculate(userLat, userLon, restLat, restLon);

                scoredPlates.Add((plato, matchScore, matchPercentage, distanceKm));
            }

            // ── Paso 6: Pipeline LINQ final — ordenar y proyectar al ViewModel ──
            var recommendations = scoredPlates
                .OrderByDescending(sp => sp.Score)
                .Take(3)
                .Select(sp => new PlatoRecommendationViewModel
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
