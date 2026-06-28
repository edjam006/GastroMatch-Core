using GastroMatch_Core.Models;

namespace GastroMatch_Core.Services.Filters
{
    /// <summary>
    /// Estrategia de filtrado por límite de precio.
    /// Patrón Strategy: encapsula la regla de exclusión por precio máximo.
    /// Principio SRP: única responsabilidad de validar el rango de precio.
    /// 
    /// Comportamiento preservado:
    /// - Excluye platos cuyo precio excede el RangoPrecioMax del usuario.
    /// - Retorna IsExcluded=true para platos fuera de presupuesto.
    /// </summary>
    public class PriceBoundaryFilter : IRecommendationFilter
    {
        public FilterResult Apply(Plato plato, PreferenciaUsuario preferencia, decimal currentScore)
        {
            if (plato.Precio > preferencia.RangoPrecioMax)
            {
                return new FilterResult(0.0m, IsExcluded: true);
            }

            return new FilterResult(currentScore, IsExcluded: false);
        }
    }
}
