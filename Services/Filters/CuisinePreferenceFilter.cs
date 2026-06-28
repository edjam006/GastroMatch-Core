using System;
using GastroMatch_Core.Models;

namespace GastroMatch_Core.Services.Filters
{
    /// <summary>
    /// Estrategia de filtrado por preferencia de tipo de cocina.
    /// Patrón Strategy: encapsula la regla de penalización por cocina no coincidente.
    /// Principio SRP: única responsabilidad de evaluar afinidad culinaria.
    /// 
    /// Comportamiento preservado:
    /// - Si el restaurante tiene un tipo de cocina definido y NO coincide con la favorita del usuario,
    ///   aplica una penalización suave del -30% sobre la puntuación actual.
    /// - La puntuación nunca cae por debajo de 0.
    /// </summary>
    public class CuisinePreferenceFilter : IRecommendationFilter
    {
        /// <summary>
        /// Penalización porcentual cuando la cocina del restaurante no coincide con la preferida.
        /// </summary>
        private const decimal CuisineMismatchPenalty = 0.30m;

        public FilterResult Apply(Plato plato, PreferenciaUsuario preferencia, decimal currentScore)
        {
            string? favCuisine = preferencia.TipoCocinaFavorita;
            string? restCuisine = plato.Restaurante?.TipoCocina;

            if (!string.IsNullOrEmpty(restCuisine))
            {
                bool matchesCuisine = !string.IsNullOrEmpty(favCuisine) &&
                                     favCuisine.Contains(restCuisine, StringComparison.OrdinalIgnoreCase);

                if (!matchesCuisine)
                {
                    currentScore -= CuisineMismatchPenalty; // Penalización suave por cocina del -30%
                }
            }

            // Aseguramos que la afinidad nunca sea negativa
            return new FilterResult(Math.Max(0.0m, currentScore), IsExcluded: false);
        }
    }
}
