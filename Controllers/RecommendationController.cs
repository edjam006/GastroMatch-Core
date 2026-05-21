using Microsoft.AspNetCore.Mvc;
using GastroMatch_Core.Models;
using GastroMatch_Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace GastroMatch_Core.Controllers
{
    public class RecommendationController : Controller
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpPost]
        public IActionResult ProcesarPreferencias(decimal RangoPrecioMax, List<string> tipoCocina, List<string> salud)
        {
            // Mapeamos los datos del formulario al modelo de PreferenciaUsuario
            var preferencias = new PreferenciaUsuario
            {
                RangoPrecioMax = RangoPrecioMax,
                TipoCocinaFavorita = tipoCocina != null && tipoCocina.Any() ? string.Join(", ", tipoCocina) : string.Empty,
                Alergias = salud != null && salud.Any() ? string.Join(", ", salud) : string.Empty,
                RestriccionesDieteticas = salud != null && salud.Any() ? string.Join(", ", salud) : string.Empty,
                UsuarioId = 1 // Mock de ID de usuario para cumplir con las llaves foráneas
            };

            // Obtenemos el Top 3 de platos recomendados respetando filtros e intolerancias
            var topPicks = _recommendationService.GetTopPicks(preferencias);

            // Creamos una lista estructurada con el plato y su porcentaje de afinidad correspondiente
            var resultList = new List<PlatoRecommendationViewModel>();
            
            foreach (var plato in topPicks)
            {
                decimal matchPercentage = _recommendationService.CalculateMatch(preferencias, plato) * 100m;
                
                // Distancia de entrega en kilómetros calculada de forma dinámica
                double simulatedDistance = 0.6 + (plato.IdPlato % 4) * 0.5;

                resultList.Add(new PlatoRecommendationViewModel
                {
                    Plato = plato,
                    MatchPercentage = (int)matchPercentage,
                    DistanceKm = simulatedDistance
                });
            }

            ViewBag.TopPicks = resultList;
            ViewBag.Preferencias = preferencias;

            return View("~/Views/Home/Index.cshtml");
        }
    }

    public class PlatoRecommendationViewModel
    {
        public Plato Plato { get; set; } = null!;
        public int MatchPercentage { get; set; }
        public double DistanceKm { get; set; }
    }
}
