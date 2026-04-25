FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Locator.ApiGateway/ApiGateway.Web/ApiGateway.Web.csproj", "src/Locator.ApiGateway/ApiGateway.Web/"]
COPY ["src/Locator.Core/Framework/Framework.csproj", "src/Locator.Core/Framework/"]
COPY ["src/Locator.Core/Shared/Shared.csproj", "src/Locator.Core/Shared/"]
RUN dotnet restore "src/Locator.ApiGateway/ApiGateway.Web/ApiGateway.Web.csproj"
COPY . .
WORKDIR "/src/src/Locator.ApiGateway/ApiGateway.Web"
RUN dotnet build "./ApiGateway.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApiGateway.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.Web.dll"]
