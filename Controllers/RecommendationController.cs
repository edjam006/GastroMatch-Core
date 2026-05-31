using Microsoft.AspNetCore.Mvc;
using GastroMatch_Core.Models;
using GastroMatch_Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
            // Recuperar el ID del usuario autenticado desde los Claims de seguridad
            int usuarioId = 1;
            var idClaim = User.FindFirst("IdUsuario");
            if (idClaim != null && int.TryParse(idClaim.Value, out int parsedId))
            {
                usuarioId = parsedId;
            }

            // Mapeamos los datos del formulario al modelo de PreferenciaUsuario
            var preferencias = new PreferenciaUsuario
            {
                RangoPrecioMax = RangoPrecioMax,
                TipoCocinaFavorita = tipoCocina != null && tipoCocina.Any() ? string.Join(", ", tipoCocina) : string.Empty,
                Alergias = salud != null && salud.Any() ? string.Join(", ", salud) : string.Empty,
                RestriccionesDieteticas = salud != null && salud.Any() ? string.Join(", ", salud) : string.Empty,
                UsuarioId = usuarioId
            };

            // El servicio retorna directamente los ViewModels listos para la vista
            var recomendaciones = _recommendationService.GetTopPicks(preferencias);
            ViewBag.TopPicks = recomendaciones;
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
