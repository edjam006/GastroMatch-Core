using System.Collections.Generic;
using GastroMatch_Core.Models;

namespace GastroMatch_Core.Services.Filters
{
    /// <summary>
    /// Abstracción para la fábrica de filtros de recomendación.
    /// Principio DIP: el motor de recomendaciones depende de esta interfaz, no de la fábrica concreta.
    /// Patrón Factory Method: define el contrato para ensamblar el pipeline de filtros.
    /// </summary>
    public interface IRecommendationFilterFactory
    {
        /// <summary>
        /// Ensambla y retorna la colección ordenada de filtros de recomendación
        /// activos según el perfil del usuario.
        /// </summary>
        /// <param name="preferencia">Preferencias del usuario para determinar qué filtros activar.</param>
        /// <returns>Lista ordenada e inmutable de filtros a ejecutar en el pipeline.</returns>
        IReadOnlyList<IRecommendationFilter> CreateFilters(PreferenciaUsuario preferencia);
    }
}
