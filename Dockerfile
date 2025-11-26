# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# CORRECCIÓN 1: Copiamos el .csproj directamente a la raíz del entorno de trabajo (/src)
# En lugar de buscar "ExamenFinal/ExamenFinal.csproj", buscamos solo el archivo asumiendo que estamos en la carpeta del proyecto.
COPY ["ExamenFinal.csproj", "./"]
RUN dotnet restore "./ExamenFinal.csproj"

# CORRECCIÓN 2: Copiamos todo el contenido del directorio actual
COPY . .

# CORRECCIÓN 3: Ya no necesitamos cambiar de directorio con WORKDIR "/src/ExamenFinal"
# porque ya copiamos todo en la raíz de /src. Compilamos directamente.
RUN dotnet build "ExamenFinal.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ExamenFinal.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExamenFinal.dll"]