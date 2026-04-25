FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Locator.Ratings/Ratings.Web/Ratings.Web.csproj", "src/Locator.Ratings/Ratings.Web/"]
COPY ["src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Presenters/HeadHunter.Presenters.csproj", "src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Presenters/"]
COPY ["src/Locator.Core/Framework/Framework.csproj", "src/Locator.Core/Framework/"]
COPY ["src/Locator.Core/Shared/Shared.csproj", "src/Locator.Core/Shared/"]
COPY ["src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Contracts/HeadHunter.Contracts.csproj", "src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Contracts/"]
COPY ["src/Locator.Core/Infrastructure/HeadHunter/HeadHunter/HeadHunter.csproj", "src/Locator.Core/Infrastructure/HeadHunter/HeadHunter/"]
COPY ["src/Locator.Core/Infrastructure/Redis/Redis.Contracts/Redis.Contracts.csproj", "src/Locator.Core/Infrastructure/Redis/Redis.Contracts/"]
COPY ["src/Locator.Core/Infrastructure/Redis/Redis.Presenters/Redis.Presenters.csproj", "src/Locator.Core/Infrastructure/Redis/Redis.Presenters/"]
COPY ["src/Locator.Core/Infrastructure/Redis/Redis/Redis.csproj", "src/Locator.Core/Infrastructure/Redis/Redis/"]
COPY ["src/Locator.Ratings/Ratings.Presenters/Ratings.Presenters.csproj", "src/Locator.Ratings/Ratings.Presenters/"]
COPY ["src/Locator.Ratings/Ratings.Application/Ratings.Application.csproj", "src/Locator.Ratings/Ratings.Application/"]
COPY ["src/Locator.Ratings/Ratings.Contracts/Ratings.Contracts.csproj", "src/Locator.Ratings/Ratings.Contracts/"]
COPY ["src/Locator.Ratings/Ratings.Domain/Ratings.Domain.csproj", "src/Locator.Ratings/Ratings.Domain/"]
COPY ["src/Locator.Ratings/Ratings.Infrastructure.Postgresql/Ratings.Infrastructure.Postgresql.csproj", "src/Locator.Ratings/Ratings.Infrastructure.Postgresql/"]
RUN dotnet restore "src/Locator.Ratings/Ratings.Web/Ratings.Web.csproj"
COPY . .
WORKDIR "/src/src/Locator.Ratings/Ratings.Web"
RUN dotnet build "./Ratings.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ratings.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ratings.Web.dll"]
