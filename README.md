# GastroMatch Core

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat-square&logo=postgresql&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-blue?style=flat-square)
![Entity Framework Core](https://img.shields.io/badge/Entity_Framework_Core-Npgsql-orange?style=flat-square)

GastroMatch Core es una aplicación web ASP.NET Core 8 que proporciona un backend robusto y un frontend MVC para la plataforma GastroMatch. Cuenta con integración a PostgreSQL a través de Entity Framework Core, autenticación basada en cookies y un servicio de recomendación personalizado.

## Tecnologías Utilizadas

*   **Framework:** .NET 8 (ASP.NET Core MVC)
*   **Base de datos:** PostgreSQL
*   **ORM:** Entity Framework Core (Npgsql)
*   **Autenticación:** Autenticación por Cookies

## Requisitos Previos

*   [SDK de .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [PostgreSQL](https://www.postgresql.org/download/)
*   (Opcional) Docker para despliegue en contenedores

## Configuración

1.  Clona el repositorio.
2.  Actualiza la cadena de conexión de la base de datos en `appsettings.json` o `appsettings.Development.json` bajo la clave `ConnectionStrings:DefaultConnection`, o establece la variable de entorno `DATABASE_URL`.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=GastroMatchDB;Username=postgres;Password=tu_contraseña"
  }
}
```

## Ejecución de la Aplicación

### Localmente mediante la CLI de .NET

1.  Navega al directorio de la solución.
2.  Restaura las dependencias:
    ```bash
    dotnet restore
    ```
3.  Aplica las migraciones de la base de datos (si aplica):
    ```bash
    dotnet ef database update
    ```
4.  Ejecuta la aplicación:
    ```bash
    dotnet run
    ```

### Usando Docker

Se incluye un archivo `Dockerfile` en el directorio raíz para construir una versión en contenedores de la aplicación.

## Estructura del Proyecto

*   `Controllers/`: Contiene los controladores MVC que manejan las peticiones HTTP entrantes.
*   `Models/`: Contiene los modelos de datos y ViewModels.
*   `Views/`: Contiene las vistas Razor para la interfaz web.
*   `Services/`: Contiene servicios de lógica de negocio, como el `RecommendationService`.
*   `Data/`: Contiene el contexto de base de datos de Entity Framework Core (`AppDbContext`).

## Características

*   **Sistema de Recomendación:** Incluye un servicio `IRecommendationService` para sugerir coincidencias.
*   **Autenticación Segura:** Utiliza la autenticación por cookies de ASP.NET Core con rutas configuradas para el inicio de sesión y acceso denegado.
*   **Listo para Múltiples Entornos:** Diseñado para funcionar sin configuración adicional en diferentes entornos (Desarrollo, Producción) a través de variables de entorno.

---

## Arquitectura de Software — Principios SOLID y Patrones de Diseño

El motor de recomendaciones de GastroMatch-Core ha sido diseñado aplicando rigurosamente **3 principios SOLID** y **3 patrones de diseño GoF**, garantizando un código extensible, mantenible y profesional sin alterar la lógica de negocio original ni las entidades del modelo de datos.

### Diagrama Arquitectónico del Motor de Recomendaciones

```
┌─────────────────────────────────────────────────────────────────┐
│                    RecommendationController                     │
│                    (depende de IRecommendationService)           │
└───────────────────────────┬─────────────────────────────────────┘
                            │ inyecta vía DI
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│              RecommendationService  [FACADE]                    │
│                                                                 │
│  Orquesta:                                                      │
│  ┌──────────────────┐  ┌──────────────────────────────────────┐ │
│  │ IDistanceCalc.   │  │ IRecommendationFilterFactory         │ │
│  │ (Haversine)      │  │ [FACTORY METHOD]                     │ │
│  └──────────────────┘  └──────────┬───────────────────────────┘ │
│                                   │ crea pipeline               │
│                                   ▼                             │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │         Pipeline de Filtros [STRATEGY PATTERN]             │ │
│  │                                                            │ │
│  │  ┌─────────────────┐ ┌──────────────────┐ ┌─────────────┐ │ │
│  │  │ HealthRestrict. │→│ PriceBoundary    │→│ CuisinePref.│ │ │
│  │  │ Filter          │ │ Filter           │ │ Filter      │ │ │
│  │  │ (early exit)    │ │ (exclusión)      │ │ (-30% pen.) │ │ │
│  │  └─────────────────┘ └──────────────────┘ └─────────────┘ │ │
│  └────────────────────────────────────────────────────────────┘ │
│                                                                 │
│  Pipeline LINQ: .OrderByDescending(score) → .Take(3) → ViewModel│
└─────────────────────────────────────────────────────────────────┘
```

---

### 1. Principios SOLID Aplicados

#### 1.1 SRP — Single Responsibility Principle (Principio de Responsabilidad Única)

> *"Una clase debe tener una, y solo una, razón para cambiar."* — Robert C. Martin

**Problema original:** La clase `RecommendationService` concentraba múltiples responsabilidades: cálculo geográfico (Haversine), validación de alergias, penalización de cocina, filtrado de precios y orquestación del pipeline.

**Solución aplicada:** Cada responsabilidad fue extraída a su propia clase especializada:

| Clase | Responsabilidad Única | Archivo |
|-------|----------------------|---------|
| `HaversineDistanceCalculator` | Cálculo de distancia geográfica | `Services/HaversineDistanceCalculator.cs` |
| `HealthRestrictionFilter` | Validación de seguridad sanitaria (alergias/restricciones) | `Services/Filters/HealthRestrictionFilter.cs` |
| `CuisinePreferenceFilter` | Evaluación de afinidad culinaria | `Services/Filters/CuisinePreferenceFilter.cs` |
| `PriceBoundaryFilter` | Validación de límite de precio | `Services/Filters/PriceBoundaryFilter.cs` |
| `RecommendationFilterFactory` | Ensamblaje del pipeline de filtros | `Services/Filters/RecommendationFilterFactory.cs` |
| `RecommendationService` | Orquestación del flujo de recomendación (Fachada) | `Services/RecommendationService.cs` |

**Evidencia en código** — Antes, el cálculo Haversine estaba embebido como método estático privado dentro del servicio. Ahora vive en su propia clase:

```csharp
// Services/HaversineDistanceCalculator.cs
public class HaversineDistanceCalculator : IDistanceCalculator
{
    private const double EarthRadiusKm = 6371.0;

    public double Calculate(double lat1, double lon1, double lat2, double lon2)
    {
        // Fórmula de Haversine — lógica original preservada intacta
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2.0) * Math.Sin(dLat / 2.0) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2.0) * Math.Sin(dLon / 2.0);
        double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
        return EarthRadiusKm * c;
    }
}
```

---

#### 1.2 OCP — Open/Closed Principle (Principio Abierto/Cerrado)

> *"Las entidades de software deben estar abiertas a la extensión, pero cerradas a la modificación."* — Bertrand Meyer

**Problema original:** Las reglas de filtrado estaban hardcodeadas dentro de un bloque `foreach` monolítico en `CalculateMatch()`. Agregar una nueva regla (ej: filtro por calorías) requería modificar directamente el método existente.

**Solución aplicada:** Cada regla de negocio implementa la interfaz `IRecommendationFilter`. Para añadir una nueva regla, basta con:
1. Crear una nueva clase que implemente `IRecommendationFilter`.
2. Registrarla en `RecommendationFilterFactory.CreateFilters()`.

**No se modifica ningún filtro existente ni el motor de recomendaciones.**

```csharp
// Services/Filters/IRecommendationFilter.cs — Contrato extensible
public interface IRecommendationFilter
{
    FilterResult Apply(Plato plato, PreferenciaUsuario preferencia, decimal currentScore);
}

// Ejemplo de extensión futura (NO requiere modificar código existente):
// public class CalorieFilter : IRecommendationFilter { ... }
```

---

#### 1.3 DIP — Dependency Inversion Principle (Principio de Inversión de Dependencias)

> *"Los módulos de alto nivel no deben depender de módulos de bajo nivel. Ambos deben depender de abstracciones."* — Robert C. Martin

**Problema original:** `RecommendationService` instanciaba directamente el cálculo Haversine como método estático interno, creando un acoplamiento fuerte imposible de testear unitariamente.

**Solución aplicada:** Todas las dependencias del servicio son abstracciones inyectadas vía constructor:

```csharp
// Services/RecommendationService.cs — Constructor con DIP puro
public class RecommendationService : IRecommendationService
{
    private readonly AppDbContext _context;
    private readonly IDistanceCalculator _distanceCalculator;           // ← Abstracción
    private readonly IRecommendationFilterFactory _filterFactory;       // ← Abstracción

    public RecommendationService(
        AppDbContext context,
        IDistanceCalculator distanceCalculator,                        // ← Inyectado
        IRecommendationFilterFactory filterFactory)                    // ← Inyectado
    { ... }
}
```

**Registro en el contenedor DI** (`Program.cs`):
```csharp
builder.Services.AddSingleton<IDistanceCalculator, HaversineDistanceCalculator>();
builder.Services.AddSingleton<IRecommendationFilterFactory, RecommendationFilterFactory>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
```

---

### 2. Patrones de Diseño Aplicados

#### 2.1 Strategy Pattern (Patrón Estrategia) — Comportamiento

> *Tipo GoF: Comportamiento. Define una familia de algoritmos encapsulados e intercambiables.*

**Aplicación:** Cada regla de negocio del motor de recomendaciones es una estrategia concreta que implementa `IRecommendationFilter`:

| Estrategia | Archivo | Comportamiento |
|------------|---------|---------------|
| `HealthRestrictionFilter` | `Services/Filters/HealthRestrictionFilter.cs` | Tokeniza alergias/restricciones, compara contra nombre, descripción e ingredientes del plato. **Early exit** (`IsExcluded=true`) si detecta un alérgeno. |
| `CuisinePreferenceFilter` | `Services/Filters/CuisinePreferenceFilter.cs` | Penalización del **-30%** si la cocina del restaurante no coincide con la favorita del usuario. Score nunca cae por debajo de 0. |
| `PriceBoundaryFilter` | `Services/Filters/PriceBoundaryFilter.cs` | Exclusión inmediata si `Precio > RangoPrecioMax`. |

**Resultado inmutable del filtro** (`record` de C# para diseño funcional):
```csharp
// Services/Filters/FilterResult.cs
public record FilterResult(decimal Score, bool IsExcluded);
```

---

#### 2.2 Factory Method (Método de Fábrica) — Creacional

> *Tipo GoF: Creacional. Define una interfaz para crear objetos, delegando la instanciación a subclases o métodos especializados.*

**Aplicación:** La clase `RecommendationFilterFactory` centraliza la creación y el orden de ejecución de las estrategias:

```csharp
// Services/Filters/RecommendationFilterFactory.cs
public class RecommendationFilterFactory : IRecommendationFilterFactory
{
    public IReadOnlyList<IRecommendationFilter> CreateFilters(PreferenciaUsuario preferencia)
    {
        var filters = new List<IRecommendationFilter>();

        filters.Add(new HealthRestrictionFilter());   // 1. Seguridad sanitaria (early exit)
        filters.Add(new PriceBoundaryFilter());       // 2. Validación de presupuesto
        filters.Add(new CuisinePreferenceFilter());   // 3. Afinidad culinaria

        return filters.AsReadOnly();
    }
}
```

El orden del pipeline es **crítico**: los filtros de exclusión se ejecutan primero para maximizar los early exits y evitar cálculos innecesarios.

---

#### 2.3 Facade Pattern (Patrón Fachada) — Estructural

> *Tipo GoF: Estructural. Proporciona una interfaz simplificada a un subsistema complejo.*

**Aplicación:** `RecommendationService` actúa como Fachada que oculta la complejidad del motor de recomendaciones detrás del método `GetTopPicks()`:

```csharp
// Services/RecommendationService.cs — Flujo orquestado por la Fachada
public List<PlatoRecommendationViewModel> GetTopPicks(PreferenciaUsuario preferencias)
{
    // Paso 1: Obtener coordenadas del usuario (fallback: Quito)
    // Paso 2: Cargar platos con Include(Restaurante, Ingredientes)
    // Paso 3: Ensamblar pipeline vía Factory Method
    var filters = _filterFactory.CreateFilters(preferencias);
    // Paso 4: Ejecutar pipeline Strategy sobre cada plato
    // Paso 5: Calcular distancia vía IDistanceCalculator (DIP)
    // Paso 6: Pipeline LINQ → OrderByDescending → Take(3) → ViewModel
}
```

El Controller (`RecommendationController`) interactúa únicamente con la interfaz `IRecommendationService`, sin conocer ningún detalle de implementación del motor interno.

---

### Estructura de Archivos del Motor Refactorizado

```
Services/
├── IRecommendationService.cs          ← Interfaz pública del motor (sin cambios)
├── RecommendationService.cs           ← FACADE — orquestador de alto nivel
├── IDistanceCalculator.cs             ← Abstracción de cálculo geográfico (DIP)
├── HaversineDistanceCalculator.cs     ← Implementación Haversine (SRP)
└── Filters/
    ├── IRecommendationFilter.cs       ← Contrato Strategy (OCP)
    ├── FilterResult.cs                ← Record inmutable de resultado
    ├── HealthRestrictionFilter.cs     ← STRATEGY — seguridad sanitaria
    ├── CuisinePreferenceFilter.cs     ← STRATEGY — afinidad culinaria (-30%)
    ├── PriceBoundaryFilter.cs         ← STRATEGY — límite de precio
    ├── IRecommendationFilterFactory.cs← Abstracción de la fábrica (DIP)
    └── RecommendationFilterFactory.cs ← FACTORY METHOD — ensamblaje del pipeline
```
