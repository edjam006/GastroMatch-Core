using System.Collections.Generic;
using GastroMatch_Core.Models;

namespace GastroMatch_Core.Services.Filters
{
    /// <summary>
    /// Fábrica concreta que ensambla el pipeline de filtros de recomendación.
    /// Patrón Factory Method: centraliza la creación y el orden de ejecución de las estrategias.
    /// Principio OCP: para agregar un nuevo filtro, solo se crea una nueva clase IRecommendationFilter
    /// y se registra aquí — sin modificar los filtros existentes ni el motor de recomendaciones.
    /// 
    /// Orden del pipeline:
    /// 1. HealthRestrictionFilter — Seguridad sanitaria (early exit por alérgenos). SIEMPRE activo.
    /// 2. PriceBoundaryFilter — Validación de presupuesto. SIEMPRE activo.
    /// 3. CuisinePreferenceFilter — Afinidad culinaria con penalización. SIEMPRE activo.
    /// </summary>
    public class RecommendationFilterFactory : IRecommendationFilterFactory
    {
        /// <summary>
        /// Ensambla dinámicamente la colección de filtros según el perfil del usuario.
        /// El orden es crítico: los filtros de exclusión (salud, precio) se ejecutan primero
        /// para maximizar los early exits y evitar cálculos innecesarios.
        /// </summary>
        public IReadOnlyList<IRecommendationFilter> CreateFilters(PreferenciaUsuario preferencia)
        {
            var filters = new List<IRecommendationFilter>();

            // 1. Filtro de seguridad sanitaria — siempre activo (obligatorio por normativa de salud)
            filters.Add(new HealthRestrictionFilter());

            // 2. Filtro de límite de precio — siempre activo
            filters.Add(new PriceBoundaryFilter());

            // 3. Filtro de preferencia de cocina — siempre activo
            filters.Add(new CuisinePreferenceFilter());

            return filters.AsReadOnly();
        }
    }
}
