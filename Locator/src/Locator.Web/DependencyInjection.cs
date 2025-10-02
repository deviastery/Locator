using Locator.Application;
using Locator.Infrastructure.Postgresql;
using Locator.Presenters;

namespace Locator.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration) =>
    services.AddWebDependencies()
            .AddApplication()
            .AddPostgresInfrastructure(configuration);

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}