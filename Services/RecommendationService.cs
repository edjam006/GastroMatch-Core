using System;
using System.Collections.Generic;
using System.Linq;
using GastroMatch_Core.Models;

namespace GastroMatch_Core.Services
{
    public class RecommendationService : IRecommendationService
    {
        public decimal CalculateMatch(PreferenciaUsuario preferencia, Plato plato)
        {
            if (preferencia == null || plato == null)
                return 0.0m;

            decimal matchScore = 1.0m; // Match base de 100%

            // 1. Descarte crítico por salud (Alergias y Restricciones Dietéticas)
            // Evaluamos intolerancias al Gluten, Lactosa y dieta Vegana de forma proactiva.
            bool userHasGlutenSensitivity = 
                preferencia.Alergias?.Contains("Gluten", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.RestriccionesDieteticas?.Contains("Gluten", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.Alergias?.Contains("Sin Gluten", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.RestriccionesDieteticas?.Contains("Sin Gluten", StringComparison.OrdinalIgnoreCase) == true;

            bool userHasLactoseSensitivity = 
                preferencia.Alergias?.Contains("Lactosa", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.RestriccionesDieteticas?.Contains("Lactosa", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.Alergias?.Contains("Sin Lactosa", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.RestriccionesDieteticas?.Contains("Sin Lactosa", StringComparison.OrdinalIgnoreCase) == true;

            bool userIsVegan = 
                preferencia.Alergias?.Contains("Vegano", StringComparison.OrdinalIgnoreCase) == true ||
                preferencia.RestriccionesDieteticas?.Contains("Vegano", StringComparison.OrdinalIgnoreCase) == true;

            // Analizamos el plato para ver si contiene Gluten
            bool platoContainsGluten = 
                plato.NombrePlato.Contains("Pan", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Pasta", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("crutones", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("brioche", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("fettuccine", StringComparison.OrdinalIgnoreCase) ||
                plato.Ingredientes.Any(i => i.NombreIngr.Contains("Gluten", StringComparison.OrdinalIgnoreCase) || 
                                             i.NombreIngr.Contains("Pan", StringComparison.OrdinalIgnoreCase) ||
                                             i.NombreIngr.Contains("Crutones", StringComparison.OrdinalIgnoreCase) ||
                                             i.NombreIngr.Contains("Fettuccine", StringComparison.OrdinalIgnoreCase));

            // Analizamos el plato para ver si contiene Lactosa
            bool platoContainsLactose = 
                plato.Descripcion.Contains("queso", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("crema", StringComparison.OrdinalIgnoreCase) ||
                plato.Ingredientes.Any(i => i.NombreIngr.Contains("Lactosa", StringComparison.OrdinalIgnoreCase) || 
                                             i.NombreIngr.Contains("Queso", StringComparison.OrdinalIgnoreCase) ||
                                             i.NombreIngr.Contains("Crema", StringComparison.OrdinalIgnoreCase));

            // Analizamos el plato para ver si contiene productos de origen animal
            bool platoContainsAnimalProducts = 
                platoContainsLactose ||
                plato.NombrePlato.Contains("Tacos", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Hamburguesa", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Pollo", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Cerdo", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Res", StringComparison.OrdinalIgnoreCase) ||
                plato.NombrePlato.Contains("Salmón", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("pollo", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("cerdo", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("salmón", StringComparison.OrdinalIgnoreCase) ||
                plato.Descripcion.Contains("res", StringComparison.OrdinalIgnoreCase) ||
                plato.Ingredientes.Any(i => i.NombreIngr.Contains("Pollo", StringComparison.OrdinalIgnoreCase) ||
                                            i.NombreIngr.Contains("Cerdo", StringComparison.OrdinalIgnoreCase) ||
                                            i.NombreIngr.Contains("Carne", StringComparison.OrdinalIgnoreCase) ||
                                            i.NombreIngr.Contains("Salmón", StringComparison.OrdinalIgnoreCase) ||
                                            i.NombreIngr.Contains("Res", StringComparison.OrdinalIgnoreCase));

            // Evaluamos descartes drásticos de salud
            if (userHasGlutenSensitivity && platoContainsGluten) return 0.0m;
            if (userHasLactoseSensitivity && platoContainsLactose) return 0.0m;
            if (userIsVegan && platoContainsAnimalProducts) return 0.0m;

            // Filtro general por coincidencias de texto de alergias específicas en el plato o sus ingredientes
            if (!string.IsNullOrEmpty(preferencia.Alergias))
            {
                var allergies = preferencia.Alergias.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var allergy in allergies)
                {
                    if (plato.NombrePlato.Contains(allergy, StringComparison.OrdinalIgnoreCase) ||
                        plato.Descripcion.Contains(allergy, StringComparison.OrdinalIgnoreCase) ||
                        plato.Ingredientes.Any(i => i.NombreIngr.Contains(allergy, StringComparison.OrdinalIgnoreCase)))
                    {
                        return 0.0m;
                    }
                }
            }

            // Filtro general por coincidencias de restricciones dietéticas adicionales
            if (!string.IsNullOrEmpty(preferencia.RestriccionesDieteticas))
            {
                var restrictions = preferencia.RestriccionesDieteticas.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var restriction in restrictions)
                {
                    if (plato.NombrePlato.Contains(restriction, StringComparison.OrdinalIgnoreCase) ||
                        plato.Descripcion.Contains(restriction, StringComparison.OrdinalIgnoreCase) ||
                        plato.Ingredientes.Any(i => i.NombreIngr.Contains(restriction, StringComparison.OrdinalIgnoreCase)))
                    {
                        return 0.0m;
                    }
                }
            }

            // 2. Coincidencia de tipo de cocina (Si coincide se mantiene alto; si no, penalización del 30%)
            bool matchesCuisine = false;
            string favCuisine = preferencia.TipoCocinaFavorita;
            
            if (!string.IsNullOrEmpty(favCuisine))
            {
                if (favCuisine.Contains("Italiana", StringComparison.OrdinalIgnoreCase) && 
                    (plato.NombrePlato.Contains("Pasta", StringComparison.OrdinalIgnoreCase) || 
                     plato.NombrePlato.Contains("César", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("Alfredo", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("parmesano", StringComparison.OrdinalIgnoreCase) ||
                     plato.Descripcion.Contains("italiana", StringComparison.OrdinalIgnoreCase)))
                {
                    matchesCuisine = true;
                }
                else if (favCuisine.Contains("Mexicana", StringComparison.OrdinalIgnoreCase) && 
                    (plato.NombrePlato.Contains("Tacos", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("cerdo marinado", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("tortillas", StringComparison.OrdinalIgnoreCase) ||
                     plato.Descripcion.Contains("mexicana", StringComparison.OrdinalIgnoreCase)))
                {
                    matchesCuisine = true;
                }
                else if (favCuisine.Contains("Asiática", StringComparison.OrdinalIgnoreCase) && 
                    (plato.NombrePlato.Contains("Sushi", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("nori", StringComparison.OrdinalIgnoreCase) || 
                     plato.Descripcion.Contains("sushi", StringComparison.OrdinalIgnoreCase) ||
                     plato.Descripcion.Contains("asiático", StringComparison.OrdinalIgnoreCase)))
                {
                    matchesCuisine = true;
                }
            }

            if (!matchesCuisine)
            {
                matchScore -= 0.30m; // Penalización del 30% si no coincide con su cocina favorita
            }

            // Aseguramos que la afinidad nunca sea negativa
            return Math.Max(0.0m, matchScore);
        }

        public List<Plato> GetTopPicks(PreferenciaUsuario preferencias)
        {
            if (preferencias == null)
                return new List<Plato>();

            // Instanciamos 5 platos simulados altamente realistas y detallados
            var mockPlates = new List<Plato>
            {
                new Plato
                {
                    IdPlato = 1,
                    NombrePlato = "Tacos Al Pastor",
                    Precio = 8.50m,
                    Calorias = 650,
                    Descripcion = "Deliciosos tacos de cerdo marinado con piña y cilantro en tortillas de maíz tradicionales. Auténtico sabor mexicano.",
                    RestauranteId = 1,
                    Ingredientes = new List<Ingrediente>
                    {
                        new Ingrediente { IdIngr = 1, NombreIngr = "Cerdo", EsAlergeno = false },
                        new Ingrediente { IdIngr = 2, NombreIngr = "Piña", EsAlergeno = false },
                        new Ingrediente { IdIngr = 3, NombreIngr = "Cilantro", EsAlergeno = false },
                        new Ingrediente { IdIngr = 4, NombreIngr = "Tortilla de maíz", EsAlergeno = false }
                    }
                },
                new Plato
                {
                    IdPlato = 2,
                    NombrePlato = "Sushi Roll Dragon",
                    Precio = 12.00m,
                    Calorias = 450,
                    Descripcion = "Rollo de sushi premium con salmón fresco, aguacate, queso crema y algas nori. Exquisitez asiática.",
                    RestauranteId = 2,
                    Ingredientes = new List<Ingrediente>
                    {
                        new Ingrediente { IdIngr = 5, NombreIngr = "Salmón", EsAlergeno = false },
                        new Ingrediente { IdIngr = 6, NombreIngr = "Aguacate", EsAlergeno = false },
                        new Ingrediente { IdIngr = 7, NombreIngr = "Queso crema (Lactosa)", EsAlergeno = true },
                        new Ingrediente { IdIngr = 8, NombreIngr = "Alga nori", EsAlergeno = false }
                    }
                },
                new Plato
                {
                    IdPlato = 3,
                    NombrePlato = "Ensalada César con Pollo",
                    Precio = 7.00m,
                    Calorias = 320,
                    Descripcion = "Lechuga romana fresca, crutones crujientes, aderezo César y queso parmesano. Ligereza de la cocina italiana.",
                    RestauranteId = 3,
                    Ingredientes = new List<Ingrediente>
                    {
                        new Ingrediente { IdIngr = 9, NombreIngr = "Lechuga", EsAlergeno = false },
                        new Ingrediente { IdIngr = 10, NombreIngr = "Crutones (Gluten)", EsAlergeno = true },
                        new Ingrediente { IdIngr = 11, NombreIngr = "Queso parmesano (Lactosa)", EsAlergeno = true },
                        new Ingrediente { IdIngr = 12, NombreIngr = "Aderezo César", EsAlergeno = false }
                    }
                },
                new Plato
                {
                    IdPlato = 4,
                    NombrePlato = "Hamburguesa Gourmet Doble",
                    Precio = 9.50m,
                    Calorias = 850,
                    Descripcion = "Hamburguesa artesanal con carne de res premium, pan brioche, queso cheddar fundido y aderezo especial de la casa.",
                    RestauranteId = 4,
                    Ingredientes = new List<Ingrediente>
                    {
                        new Ingrediente { IdIngr = 13, NombreIngr = "Carne de res", EsAlergeno = false },
                        new Ingrediente { IdIngr = 14, NombreIngr = "Pan brioche (Gluten)", EsAlergeno = true },
                        new Ingrediente { IdIngr = 15, NombreIngr = "Queso cheddar (Lactosa)", EsAlergeno = true }
                    }
                },
                new Plato
                {
                    IdPlato = 5,
                    NombrePlato = "Pasta Alfredo Fettuccine",
                    Precio = 11.00m,
                    Calorias = 750,
                    Descripcion = "Fideos fettuccine premium en una cremosa y clásica salsa Alfredo de queso y crema italiana.",
                    RestauranteId = 5,
                    Ingredientes = new List<Ingrediente>
                    {
                        new Ingrediente { IdIngr = 16, NombreIngr = "Fettuccine (Gluten)", EsAlergeno = true },
                        new Ingrediente { IdIngr = 17, NombreIngr = "Crema de leche (Lactosa)", EsAlergeno = true },
                        new Ingrediente { IdIngr = 18, NombreIngr = "Queso parmesano (Lactosa)", EsAlergeno = true }
                    }
                }
            };

            // Recorremos los platos para calcular su puntuación de afinidad (Match Score)
            var scoredPlates = new List<(Plato Plato, decimal Score)>();
            foreach (var plato in mockPlates)
            {
                decimal matchScore = CalculateMatch(preferencias, plato);
                scoredPlates.Add((plato, matchScore));
            }

            // Aplicamos los filtros solicitados mediante LINQ:
            // 1. Filtrar por precio menor o igual al rango máximo del usuario.
            // 2. Filtrar para evitar recomendar platos con 0% de compatibilidad (descartes críticos de salud).
            // 3. Ordenar descendientemente por el Match Score.
            // 4. Retornar estrictamente el Top 3.
            var recommendations = scoredPlates
                .Where(sp => sp.Plato.Precio <= preferencias.RangoPrecioMax && sp.Score > 0m)
                .OrderByDescending(sp => sp.Score)
                .Select(sp => sp.Plato)
                .Take(3)
                .ToList();

            return recommendations;
        }
    }
}
