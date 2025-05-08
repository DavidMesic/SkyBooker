# Stage: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# netcat installieren
RUN apt-get update \
 && apt-get install -y netcat-openbsd \
 && rm -rf /var/lib/apt/lists/*

# Stage: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BookingService.csproj", "."]
RUN dotnet restore "./BookingService.csproj"
COPY . .
RUN dotnet build "./BookingService.csproj" -c Release -o /app/build

# Stage: Publish
FROM build AS publish
RUN dotnet publish "./BookingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# EntryPoint: warte auf SQL Server, dann starte
ENTRYPOINT ["sh","-c", "\
  echo 'Warte auf SQL Server...'; \
  until nc -z sqlserver 1433; do \
    echo 'SQL noch nicht erreichbar, warte 2s...'; \
    sleep 2; \
  done; \
  echo 'SQL erreichbar – starte BookingService'; \
  exec dotnet BookingService.dll \
"]
