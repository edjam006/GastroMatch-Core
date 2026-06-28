using System;

namespace GastroMatch_Core.Services
{
    /// <summary>
    /// Abstracción para el cálculo de distancia geográfica entre dos coordenadas.
    /// Principio DIP: los componentes de alto nivel dependen de esta interfaz, no de una implementación concreta.
    /// </summary>
    public interface IDistanceCalculator
    {
        /// <summary>
        /// Calcula la distancia en kilómetros entre dos puntos geográficos.
        /// </summary>
        double Calculate(double lat1, double lon1, double lat2, double lon2);
    }
}
