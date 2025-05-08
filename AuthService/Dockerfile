# File: AuthService/Dockerfile (angepasst)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Ordner für die SQLite-Datenbank anlegen und Schreibrechte für alle setzen
RUN mkdir -p /data && chmod 777 /data
VOLUME /data

# Wechsel auf den Nicht-Root-User
USER $APP_UID

# Build-Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AuthService.csproj", "."]
RUN dotnet restore "./AuthService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish-Stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Finales Image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]