using System.Text.Json.Serialization;
using Framework.DependencyInjection;
using HeadHunter.Presenters;
using Microsoft.OpenApi.Models;
using Redis.Presenters;
using Users.Presenters;

namespace Users.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration) =>
    services.AddWebDependencies()
            .AddUsers(configuration)
            .AddHeadHunter(configuration)
            .AddRedis(configuration)
    ;

    public static IApplicationBuilder UseRatingsPolicy(
        this IApplicationBuilder app)
    {
        app.UseCorePolicies();
        return app;
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();
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