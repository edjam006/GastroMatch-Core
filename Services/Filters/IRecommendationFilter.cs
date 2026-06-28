using GastroMatch_Core.Models;

namespace GastroMatch_Core.Services.Filters
{
    /// <summary>
    /// Interfaz del patrón Strategy para los filtros de recomendación.
    /// Patrón Strategy: define un contrato común que permite intercambiar algoritmos de filtrado
    /// sin modificar el motor de recomendaciones (OCP).
    /// Principio DIP: el motor depende de esta abstracción, no de implementaciones concretas.
    /// </summary>
    public interface IRecommendationFilter
    {
        /// <summary>
        /// Aplica una regla de negocio sobre un plato candidato y retorna el resultado del filtrado.
        /// </summary>
        /// <param name="plato">Plato candidato a evaluar.</param>
        /// <param name="preferencia">Preferencias del usuario autenticado.</param>
        /// <param name="currentScore">Puntuación acumulada actual del plato en el pipeline.</param>
        /// <returns>FilterResult con la puntuación ajustada y/o señal de exclusión.</returns>
        FilterResult Apply(Plato plato, PreferenciaUsuario preferencia, decimal currentScore);
    }
}
