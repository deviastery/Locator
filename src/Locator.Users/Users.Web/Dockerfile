FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Locator.Users/Users.Web/Users.Web.csproj", "src/Locator.Users/Users.Web/"]
COPY ["src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Presenters/HeadHunter.Presenters.csproj", "src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Presenters/"]
COPY ["src/Locator.Core/Framework/Framework.csproj", "src/Locator.Core/Framework/"]
COPY ["src/Locator.Core/Shared/Shared.csproj", "src/Locator.Core/Shared/"]
COPY ["src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Contracts/HeadHunter.Contracts.csproj", "src/Locator.Core/Infrastructure/HeadHunter/HeadHunter.Contracts/"]
COPY ["src/Locator.Core/Infrastructure/HeadHunter/HeadHunter/HeadHunter.csproj", "src/Locator.Core/Infrastructure/HeadHunter/HeadHunter/"]
COPY ["src/Locator.Core/Infrastructure/Redis/Redis.Contracts/Redis.Contracts.csproj", "src/Locator.Core/Infrastructure/Redis/Redis.Contracts/"]
COPY ["src/Locator.Core/Infrastructure/Redis/Redis.Presenters/Redis.Presenters.csproj", "src/Locator.Core/Infrastructure/Redis/Redis.Presenters/"]
COPY ["src/Locator.Core/Infrastructure/Redis/Redis/Redis.csproj", "src/Locator.Core/Infrastructure/Redis/Redis/"]
COPY ["src/Locator.Users/Users.Presenters/Users.Presenters.csproj", "src/Locator.Users/Users.Presenters/"]
COPY ["src/Locator.Users/Users.Application/Users.Application.csproj", "src/Locator.Users/Users.Application/"]
COPY ["src/Locator.Users/Users.Contracts/Users.Contracts.csproj", "src/Locator.Users/Users.Contracts/"]
COPY ["src/Locator.Users/Users.Domain/Users.Domain.csproj", "src/Locator.Users/Users.Domain/"]
COPY ["src/Locator.Users/Users.Infrastructure.Postgresql/Users.Infrastructure.Postgresql.csproj", "src/Locator.Users/Users.Infrastructure.Postgresql/"]
RUN dotnet restore "src/Locator.Users/Users.Web/Users.Web.csproj"
COPY . .
WORKDIR "/src/src/Locator.Users/Users.Web"
RUN dotnet build "./Users.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Users.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.Web.dll"]
