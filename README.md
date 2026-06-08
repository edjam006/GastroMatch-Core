# GastroMatch Core

GastroMatch Core is an ASP.NET Core 8 Web Application that provides a robust backend and MVC frontend for the GastroMatch platform. It features PostgreSQL integration via Entity Framework Core, Cookie-based Authentication, and a custom Recommendation Service.

## Technologies Used

*   **Framework:** .NET 8 (ASP.NET Core MVC)
*   **Database:** PostgreSQL
*   **ORM:** Entity Framework Core (Npgsql)
*   **Authentication:** Cookie Authentication

## Prerequisites

*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [PostgreSQL](https://www.postgresql.org/download/)
*   (Optional) Docker for containerized deployment

## Configuration

1.  Clone the repository.
2.  Update the database connection string in `appsettings.json` or `appsettings.Development.json` under `ConnectionStrings:DefaultConnection`, or set the `DATABASE_URL` environment variable.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=GastroMatchDB;Username=postgres;Password=your_password"
  }
}
```

## Running the Application

### Locally via .NET CLI

1.  Navigate to the project directory.
2.  Restore dependencies:
    ```bash
    dotnet restore
    ```
3.  Apply database migrations (if applicable):
    ```bash
    dotnet ef database update
    ```
4.  Run the application:
    ```bash
    dotnet run
    ```

### Using Docker

A `Dockerfile` is included in the root directory for building a containerized version of the application.

## Project Structure

*   `Controllers/`: Contains the MVC controllers handling incoming HTTP requests.
*   `Models/`: Contains the data models and ViewModels.
*   `Views/`: Contains the Razor views for the web interface.
*   `Services/`: Contains business logic services, such as the `RecommendationService`.
*   `Data/`: Contains the Entity Framework Core database context (`AppDbContext`).

## Features

*   **Recommendation System:** Includes an `IRecommendationService` for suggesting matches.
*   **Secure Authentication:** Utilizes ASP.NET Core Cookie Authentication with configured login and access denied paths.
*   **Environment Ready:** Designed to work out-of-the-box with different environments (Development, Production) and environment variable configurations.
