using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application;

namespace Users.Infrastructure.Postgresql;

public static class DependencyInjection
{
        public static IServiceCollection AddPostgresInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("UsersDb")));

        services.AddScoped<IUsersRepository, UsersEfCoreRepository>();
        
        services.AddScoped<IUsersReadDbContext>(sp => sp.GetRequiredService<UsersDbContext>());
        return services;
    }
}