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
