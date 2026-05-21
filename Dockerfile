# Etapa 1: Compilación y restauración
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar archivos de proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código y construir
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa 2: Entorno de ejecución ligero
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Render mapea automáticamente el puerto 8080 para .NET 8 de forma nativa
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GastroMatch-Core.dll"]
