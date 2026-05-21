using GastroMatch_Core.Models;
using System.Collections.Generic;

namespace GastroMatch_Core.Services
{
    public interface IRecommendationService
    {
        List<Plato> GetTopPicks(PreferenciaUsuario preferencias);

        decimal CalculateMatch(PreferenciaUsuario preferencia, Plato plato);
    }
}
