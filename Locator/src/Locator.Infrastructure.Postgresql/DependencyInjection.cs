using FluentValidation;
using Locator.Application.Ratings;
using Locator.Application.Users;
using Locator.Application.Vacancies;
using Locator.Infrastructure.Postgresql.Ratings;
using Locator.Infrastructure.Postgresql.Users;
using Locator.Infrastructure.Postgresql.Vacancies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Locator.Infrastructure.Postgresql;

public static class DependencyInjection
{
        public static IServiceCollection AddPostgresInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddDbContext<VacanciesDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("VacanciesDb")));
        services.AddDbContext<RatingsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("VacanciesDb")));
        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("VacanciesDb")));

        services.AddScoped<IVacanciesRepository, VacanciesEfCoreRepository>();
        services.AddScoped<IRatingsRepository, RatingsEfCoreRepository>();
        services.AddScoped<IUsersRepository, UsersEfCoreRepository>();
        
        services.AddScoped<IVacanciesReadDbContext>(sp => sp.GetRequiredService<VacanciesDbContext>());
        services.AddScoped<IRatingsReadDbContext>(sp => sp.GetRequiredService<RatingsDbContext>());
        services.AddScoped<IUsersReadDbContext>(sp => sp.GetRequiredService<UsersDbContext>());
        return services;
    }
}