# Stage: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# netcat installieren, damit nc im entrypoint funktioniert
RUN apt-get update \
 && apt-get install -y netcat-openbsd \
 && rm -rf /var/lib/apt/lists/*

# Stage: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MessageService.csproj", "."]
RUN dotnet restore "./MessageService.csproj"
COPY . .
RUN dotnet build "./MessageService.csproj" -c Release -o /app/build

# Stage: Publish
FROM build AS publish
RUN dotnet publish "./MessageService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh","-c", \
  "echo 'Warte auf RabbitMQ…'; \
   until nc -z rabbitmq 5672; do echo 'noch nicht da…'; sleep 2; done; \
   echo 'RabbitMQ da – starte Service'; \
   exec dotnet MessageService.dll" ]
