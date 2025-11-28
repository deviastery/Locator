using System.Text.Json.Serialization;
using Locator.Application;
using Locator.Infrastructure.HhApi;
using Locator.Infrastructure.Postgresql;
using Locator.Infrastructure.Redis;
using Microsoft.OpenApi.Models;

namespace Locator.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration) =>
    services.AddWebDependencies()
            .AddApplication(configuration)
            .AddPostgresInfrastructure(configuration)
            .AddRedisInfrastructure(configuration)
            .AddHhApiInfrastructure(configuration);

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Locator API",
                Version = "v1",
            });
        });

        return services;
    }
}