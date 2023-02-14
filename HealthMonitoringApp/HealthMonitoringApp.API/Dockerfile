#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HealthMonitoringApp.API/HealthMonitoringApp.API.csproj", "HealthMonitoringApp.API/"]
COPY ["HealthMonitoringApp.Business/HealthMonitoringApp.Business.csproj", "HealthMonitoringApp.Business/"]
COPY ["HealthMonitoringApp.Data/HealthMonitoringApp.Data.csproj", "HealthMonitoringApp.Data/"]
COPY ["HealthMonitoringApp.Application/HealthMonitoringApp.Application.csproj", "HealthMonitoringApp.Application/"]
COPY ["HealthMonitoringApp.Core/HealthMonitoringApp.Core.csproj", "HealthMonitoringApp.Core/"]
RUN dotnet restore "HealthMonitoringApp.API/HealthMonitoringApp.API.csproj"
COPY . .
WORKDIR "/src/HealthMonitoringApp.API"
RUN dotnet build "HealthMonitoringApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HealthMonitoringApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthMonitoringApp.API.dll"]