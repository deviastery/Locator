using System.Text.Json.Serialization;
using HeadHunter.Presenters;
using Microsoft.OpenApi.Models;
using Ratings.Presenters;
using Redis.Presenters;
using Users.Presenters;
using Vacancies.Presenters;

namespace Locator.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration) =>
    services.AddWebDependencies()
            .AddRatingsModule(configuration)
            .AddUsersModule(configuration)
            .AddVacanciesModule(configuration)
            .AddHeadHunterModule(configuration)
            .AddRedisModule(configuration)
    ;

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