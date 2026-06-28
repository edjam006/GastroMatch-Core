using System;

namespace GastroMatch_Core.Services
{
    /// <summary>
    /// Implementación concreta del cálculo de distancia utilizando la Fórmula de Haversine.
    /// Principio SRP: esta clase tiene la única responsabilidad de calcular distancias geográficas.
    /// Extraída del RecommendationService original para cumplir con la separación de responsabilidades.
    /// </summary>
    public class HaversineDistanceCalculator : IDistanceCalculator
    {
        /// <summary>
        /// Radio medio de la Tierra en kilómetros (constante geodésica estándar WGS-84).
        /// </summary>
        private const double EarthRadiusKm = 6371.0;

        /// <summary>
        /// Calcula la distancia en kilómetros entre dos puntos geográficos
        /// utilizando la Fórmula de Haversine, optimizada para distancias urbanas en Quito.
        /// </summary>
        public double Calculate(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2.0) * Math.Sin(dLat / 2.0) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2.0) * Math.Sin(dLon / 2.0);

            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

            return EarthRadiusKm * c;
        }

        /// <summary>
        /// Convierte grados sexagesimales a radianes.
        /// </summary>
        private static double ToRadians(double angle)
        {
            return (Math.PI / 180.0) * angle;
        }
    }
}
