using GastroMatch_Core.Models;
using GastroMatch_Core.Controllers;
using System.Collections.Generic;

namespace GastroMatch_Core.Services
{
    public interface IRecommendationService
    {
        List<PlatoRecommendationViewModel> GetTopPicks(PreferenciaUsuario preferencias);

        decimal CalculateMatch(PreferenciaUsuario preferencia, Plato plato);
    }
}
