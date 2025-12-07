using FluentValidation;
using Framework.DependencyInjection;
using Microsoft.AspNetCore.Builder;
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

        services.AddCoreServices<RatingsDbContext>(configuration, "RatingsDb");

        services.AddScoped<IRatingsRepository, RatingsEfCoreRepository>();
        
        services.AddScoped<IRatingsReadDbContext>(sp => sp.GetRequiredService<RatingsDbContext>());
        return services;
    }
}