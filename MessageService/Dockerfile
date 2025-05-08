# Stage: Runtime (production)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

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
ENTRYPOINT ["dotnet", "MessageService.dll"]
