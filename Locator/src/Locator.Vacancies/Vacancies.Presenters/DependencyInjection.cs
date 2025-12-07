using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vacancies.Application;
using Vacancies.Infrastructure.Postgresql;

namespace Vacancies.Presenters;

public static class DependencyInjection
{
    public static IServiceCollection AddVacancies(
        this IServiceCollection services,
        IConfiguration configuration) =>
        
        services.AddApplication(configuration)
            .AddPostgresInfrastructure(configuration);
}