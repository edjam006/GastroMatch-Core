namespace GastroMatch_Core.Services.Filters
{
    /// <summary>
    /// Resultado inmutable de la evaluación de un filtro de recomendación.
    /// Diseño funcional: cada filtro retorna un nuevo FilterResult sin efectos secundarios.
    /// </summary>
    /// <param name="Score">Puntuación actualizada después de aplicar el filtro (0.0m a 1.0m).</param>
    /// <param name="IsExcluded">Indica si el plato debe ser excluido definitivamente del pipeline (ej: alérgeno detectado).</param>
    public record FilterResult(decimal Score, bool IsExcluded);
}
