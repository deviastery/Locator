FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Locator.Vacancies/Vacancies.Web/Vacancies.Web.csproj", "src/Locator.Vacancies/Vacancies.Web/"]
COPY ["src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Contracts/HeadHunter.Contracts.csproj", "src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Contracts/"]
COPY ["src/Locator.Core/Shared/Shared.csproj", "src/Locator.Core/Shared/"]
COPY ["src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Presenters/HeadHunter.Presenters.csproj", "src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Presenters/"]
COPY ["src/Locator.Core/Framework/Framework.csproj", "src/Locator.Core/Framework/"]
COPY ["src/Locator.Core/Infrastructure/HeadHunter/HeadHunter/HeadHunter.csproj", "src/Locator.Core/Infrastructure/HeadHunter/HeadHunter/"]
COPY ["src/Locator.Core/Infrastructure/Redis/Redis.Contracts/Redis.Contracts.csproj", "src/Locator.Core/Infrastructure/Redis/Redis.Contracts/"]
COPY ["src/Locator.Core/Infrastructure/Redis/Redis.Presenters/Redis.Presenters.csproj", "src/Locator.Core/Infrastructure/Redis/Redis.Presenters/"]
COPY ["src/Locator.Core/Infrastructure/Redis/Redis/Redis.csproj", "src/Locator.Core/Infrastructure/Redis/Redis/"]
COPY ["src/Locator.Vacancies/Vacancies.Presenters/Vacancies.Presenters.csproj", "src/Locator.Vacancies/Vacancies.Presenters/"]
COPY ["src/Locator.Vacancies/Vacancies.Application/Vacancies.Application.csproj", "src/Locator.Vacancies/Vacancies.Application/"]
COPY ["src/Locator.Vacancies/Vacancies.Contracts/Vacancies.Contracts.csproj", "src/Locator.Vacancies/Vacancies.Contracts/"]
COPY ["src/Locator.Vacancies/Vacancies.Domain/Vacancies.Domain.csproj", "src/Locator.Vacancies/Vacancies.Domain/"]
COPY ["src/Locator.Vacancies/Vacancies.Infrastructure.Postgresql/Vacancies.Infrastructure.Postgresql.csproj", "src/Locator.Vacancies/Vacancies.Infrastructure.Postgresql/"]
RUN dotnet restore "src/Locator.Vacancies/Vacancies.Web/Vacancies.Web.csproj"
COPY . .
WORKDIR "/src/src/Locator.Vacancies/Vacancies.Web"
RUN dotnet build "./Vacancies.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Vacancies.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vacancies.Web.dll"]
