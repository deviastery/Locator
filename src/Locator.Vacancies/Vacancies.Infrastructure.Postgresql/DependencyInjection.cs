using FluentValidation;
using Framework.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vacancies.Application;

namespace Vacancies.Infrastructure.Postgresql;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgresInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        services.AddCoreServices<VacanciesDbContext>(configuration, "VacanciesDb");

        services.AddScoped<IVacanciesRepository, VacanciesEfCoreRepository>();
        
        services.AddScoped<IVacanciesReadDbContext>(sp => sp.GetRequiredService<VacanciesDbContext>());
        return services;
    }
}