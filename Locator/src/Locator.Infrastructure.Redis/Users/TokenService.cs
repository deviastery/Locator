using System.Text.Json;
using Locator.Application.Users;
using Locator.Domain.Users;
using Microsoft.Extensions.Caching.Distributed;

namespace Locator.Infrastructure.Redis.Users;

public class TokenCacheService : ITokenCacheService
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public TokenCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    
    private string GetEmployeeTokenKey(Guid userId) => $"tokens:employee:{userId}";
    private string GetRefreshTokenKey(Guid userId) => $"tokens:refresh:{userId}";
    
    public async Task SetEmployeeTokenAsync(
        string token, 
        Guid userId,
        CancellationToken cancellationToken,
        TimeSpan? expiry = null)
    {
        string json = JsonSerializer.Serialize(token, JsonOptions);
        await _cache.SetStringAsync(
            GetEmployeeTokenKey(userId),
            json,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromDays(1) },
            cancellationToken);
    }
    
    public async Task<string?> GetEmployeeTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        string? json = await _cache.GetStringAsync(GetEmployeeTokenKey(userId), cancellationToken);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<string>(json, JsonOptions);
    }
    
    public async Task SetRefreshTokenAsync(
        RefreshToken token, 
        CancellationToken cancellationToken,
        TimeSpan? expiry = null)
    {
        string json = JsonSerializer.Serialize(token, JsonOptions);
        await _cache.SetStringAsync(
            GetRefreshTokenKey(token.UserId),
            json,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromDays(1) },
            cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        string? json = await _cache.GetStringAsync(GetRefreshTokenKey(userId), cancellationToken);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<RefreshToken>(json, JsonOptions);
    }
    
    public async Task InvalidateTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(GetEmployeeTokenKey(userId), cancellationToken);
        await _cache.RemoveAsync(GetRefreshTokenKey(userId), cancellationToken);
    }
}