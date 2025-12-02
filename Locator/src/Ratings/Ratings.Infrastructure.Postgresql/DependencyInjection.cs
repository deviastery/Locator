using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ratings.Application;

namespace Ratings.Infrastructure.Postgresql;

public static class DependencyInjection
{
        public static IServiceCollection AddPostgresInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddDbContext<RatingsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("RatingsDb")));

        services.AddScoped<IRatingsRepository, RatingsEfCoreRepository>();
        
        services.AddScoped<IRatingsReadDbContext>(sp => sp.GetRequiredService<RatingsDbContext>());
        return services;
    }
}