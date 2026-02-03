using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Redis.Contracts;
using Shared.Options;
using Testcontainers.Redis;

namespace Redis.Tests.IntegrationTests;

public class DockerRedisFixture : IAsyncLifetime
{
    private RedisContainer _redisContainer;
    private ServiceProvider _serviceProvider;
    
    public DockerRedisFixture()
    {
        _redisContainer = new RedisBuilder("redis:7.0").Build();
    }

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();

        var services = new ServiceCollection();
        string? connectionString = _redisContainer.GetConnectionString();
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "Locator:";
        });
        services.Configure<TokenCacheOptions>(options =>
        {
            options.EmployeeTokenExpiryMins = "5";
            options.RefreshTokenExpiryMins = "10";
        });
        services.AddScoped<ITokenCacheContract, TokenCacheService>();
        
        _serviceProvider = services.BuildServiceProvider();
    }
    
    public ITokenCacheContract GetTokenCache()
    {
        return _serviceProvider.GetRequiredService<ITokenCacheContract>();
    }

    public async Task DisposeAsync()
    {
        await _redisContainer.StopAsync();
        await _serviceProvider.DisposeAsync();
    }
}