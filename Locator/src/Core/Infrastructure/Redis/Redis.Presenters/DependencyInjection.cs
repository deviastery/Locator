using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis.Contracts;
using Shared.Options;

namespace Redis.Presenters;

public static class DependencyInjection
{
    public static IServiceCollection AddRedisModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "Locator:";
        });
        
        services.Configure<TokenCacheOptions>(configuration
            .GetSection(TokenCacheOptions.SECTION_NAME));
        
        services.AddScoped<ITokenCacheContract, TokenCacheService>();

        return services;
    }
}