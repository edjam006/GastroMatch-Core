using System;
using System.Collections.Generic;
using System.Linq;
using GastroMatch_Core.Models;

namespace GastroMatch_Core.Services.Filters
{
    /// <summary>
    /// Estrategia de filtrado por restricciones de salud (alergias y restricciones dietéticas).
    /// Patrón Strategy: encapsula la regla de negocio de seguridad sanitaria.
    /// Principio SRP: única responsabilidad de validar compatibilidad de salud.
    /// 
    /// Comportamiento crítico preservado:
    /// - Tokeniza alergias y restricciones dietéticas del usuario.
    /// - Elimina duplicados para eficiencia.
    /// - Compara tokens contra NombrePlato, Descripcion e Ingredientes del plato.
    /// - Early exit de seguridad sanitaria: retorna IsExcluded=true si hay coincidencia.
    /// </summary>
    public class HealthRestrictionFilter : IRecommendationFilter
    {
        public FilterResult Apply(Plato plato, PreferenciaUsuario preferencia, decimal currentScore)
        {
            // 1. Construir la lista unificada de tokens de salud (alergias + restricciones)
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

            // 2. Validación crítica de seguridad: early exit si se detecta un alérgeno
            foreach (var token in healthTokens)
            {
                // Comparación de subcadena insensible a mayúsculas/minúsculas
                if (plato.NombrePlato.Contains(token, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(plato.Descripcion) && plato.Descripcion.Contains(token, StringComparison.OrdinalIgnoreCase)) ||
                    (plato.Ingredientes != null && plato.Ingredientes.Any(i => i.NombreIngr.Contains(token, StringComparison.OrdinalIgnoreCase))))
                {
                    return new FilterResult(0.0m, IsExcluded: true); // Early exit de seguridad sanitaria
                }
            }

            // Sin alérgenos detectados — el plato es seguro
            return new FilterResult(currentScore, IsExcluded: false);
        }
    }
}
