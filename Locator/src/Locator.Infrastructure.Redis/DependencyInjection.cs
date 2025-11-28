using Locator.Application.Users;
using Locator.Infrastructure.Redis.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Locator.Infrastructure.Redis;

public static class DependencyInjection
{
    public static IServiceCollection AddRedisInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "Locator:";
        });
        
        services.AddScoped<ITokenCacheService, TokenCacheService>();

        return services;
    }
}