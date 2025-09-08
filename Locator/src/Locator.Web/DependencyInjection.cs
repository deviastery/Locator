using Locator.Application;
using Locator.Infrastructure.Postgresql;

namespace Locator.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration) =>
    services.AddWebDependencies()
            .AddApplication()
            .AddInfrastructure(configuration);

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}