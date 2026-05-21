-- Limpiar tablas previas para asegurar consistencia
DELETE FROM "PlatoIngrediente";
DELETE FROM "Ingredientes";
DELETE FROM "Platos";
DELETE FROM "Restaurantes";

-- 1. Insertar Restaurantes locales reales
INSERT INTO "Restaurantes" ("IdRest", "NombreLocal", "Direccion", "Calificacion", "HorarioApertura", "HorarioCierre") VALUES
(1, 'El Rincón Mexicano', 'Av. Amazonas N24-12, Quito', 4.7, '11:00:00', '22:00:00'),
(2, 'Kyoto Sushi Bar', 'Calle Larga 8-24, Cuenca', 4.5, '12:00:00', '23:00:00'),
(3, 'Trattoria Da Luigi', 'Av. 9 de Octubre 1200, Guayaquil', 4.8, '12:00:00', '22:30:00');

-- 2. Insertar Ingredientes detallados (incluyendo alérgenos comunes)
INSERT INTO "Ingredientes" ("IdIngr", "NombreIngr", "EsAlergeno") VALUES
(1, 'Tortilla de maíz', false),
(2, 'Carne de cerdo', false),
(3, 'Piña', false),
(4, 'Cilantro', false),
(5, 'Tortilla de trigo (Gluten)', true),
(6, 'Carne de res', false),
(7, 'Frijoles', false),
(8, 'Queso cheddar (Lactosa)', true),
(9, 'Salmón', false),
(10, 'Arroz de sushi', false),
(11, 'Aguacate', false),
(12, 'Alga nori', false),
(13, 'Queso crema (Lactosa)', true),
(14, 'Langostinos', false),
(15, 'Salsa picante', false),
(16, 'Fetuccini (Gluten)', true),
(17, 'Crema de leche (Lactosa)', true),
(18, 'Queso parmesano (Lactosa)', true),
(19, 'Salsa de tomate', false),
(20, 'Albahaca', false),
(21, 'Aceite de oliva', false),
(22, 'Lechuga romana', false),
(23, 'Pechuga de pollo', false),
(24, 'Aderezo César (Lactosa)', true),
(25, 'Crutones (Gluten)', true),
(26, 'Tofu', false),
(27, 'Champiñones', false),
(28, 'Zanahoria', false),
(29, 'Maní', true);

-- 3. Insertar Platos realistas
INSERT INTO "Platos" ("IdPlato", "NombrePlato", "Precio", "Calorias", "Descripcion", "RestauranteId") VALUES
(1, 'Tacos Al Pastor', 8.50, 650, 'Tacos de cerdo marinado con piña y cilantro en tortillas de maíz.', 1),
(2, 'Burrito de Res', 9.50, 850, 'Burrito relleno de carne de res, frijoles y queso cheddar en tortilla de trigo.', 1),
(3, 'Sushi Roll Dragon', 12.00, 450, 'Rollo de sushi premium con salmón, aguacate, queso crema y algas nori.', 2),
(4, 'Sushi Vegano', 10.00, 300, 'Rollo de sushi con aguacate, zanahoria y pepino envuelto en alga nori.', 2),
(5, 'Fettuccine Alfredo', 11.00, 750, 'Fideos fettuccine en salsa Alfredo de queso y crema.', 3),
(6, 'Pizza Margherita', 9.00, 700, 'Pizza clásica con salsa de tomate, mozzarella fresca y hojas de albahaca.', 3),
(7, 'Ensalada César', 7.50, 400, 'Lechuga romana con pechuga de pollo, aderezo César y queso parmesano.', 3),
(8, 'Tacos Veganos de Tofu', 8.00, 450, 'Tacos de tofu marinado con aguacate y cilantro en tortillas de maíz.', 1),
(9, 'Ramen Asiático de Pollo', 13.00, 650, 'Sopa tradicional de ramen con fideos de trigo, caldo y pechuga de pollo.', 2),
(10, 'Pad Thai de Langostinos con Maní', 14.50, 700, 'Fideos de arroz salteados con langostinos, brotes de soja y maní triturado.', 2);

-- 4. Insertar PlatoIngrediente (Relación Muchos a Muchos)
INSERT INTO "PlatoIngrediente" ("PlatosIdPlato", "IngredientesIdIngr") VALUES
-- Tacos Al Pastor (1)
(1, 1), (1, 2), (1, 3), (1, 4),
-- Burrito de Res (2)
(2, 5), (2, 6), (2, 7), (2, 8),
-- Sushi Roll Dragon (3)
(3, 9), (3, 10), (3, 11), (3, 12), (3, 13),
-- Sushi Vegano (4)
(4, 10), (4, 11), (4, 12), (4, 28),
-- Fettuccine Alfredo (5)
(5, 16), (5, 17), (5, 18),
-- Pizza Margherita (6)
(6, 16), (6, 8), (6, 19), (6, 20), (6, 21),
-- Ensalada César (7)
(7, 22), (7, 23), (7, 24), (7, 25), (7, 18),
-- Tacos Veganos de Tofu (8)
(8, 1), (8, 26), (8, 11), (8, 4), (8, 27),
-- Ramen de Pollo (9)
(9, 16), (9, 23), (9, 12), (9, 27),
-- Pad Thai con Maní (10)
(10, 10), (10, 14), (10, 29);
