using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Redis.Contracts;
using Shared.Options;
using Users.Contracts.Dto;

namespace Redis;

public class TokenCacheService : ITokenCacheContract
{
    private readonly IDistributedCache _cache;
    private readonly TokenCacheOptions _tokenCacheOptions;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public TokenCacheService(
        IDistributedCache cache, 
        IOptions<TokenCacheOptions> tokenCacheOptions)
    {
        _cache = cache;
        _tokenCacheOptions = tokenCacheOptions.Value;
    }
    
    private string GetEmployeeTokenKey(Guid userId) => $"tokens:employee:{userId}";
    private string GetRefreshTokenKey(Guid userId) => $"tokens:refresh:{userId}";
    
    public async Task SetEmployeeTokenAsync(
        string token, 
        Guid userId,
        CancellationToken cancellationToken,
        TimeSpan? expiry = null)
    {
        _ = int.TryParse(_tokenCacheOptions.EmployeeTokenExpiryMins, out int validityMins) ? validityMins : 20160;
        string json = JsonSerializer.Serialize(token, JsonOptions);
        await _cache.SetStringAsync(
            GetEmployeeTokenKey(userId),
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(validityMins),
            },
            cancellationToken);
    }
    
    public async Task<string?> GetEmployeeTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        string? json = await _cache.GetStringAsync(GetEmployeeTokenKey(userId), cancellationToken);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<string>(json, JsonOptions);
    }
    
    public async Task SetRefreshTokenAsync(
        RefreshTokenDto tokenDto, 
        CancellationToken cancellationToken,
        TimeSpan? expiry = null)
    {
        _ = int.TryParse(_tokenCacheOptions.RefreshTokenExpiryMins, out int validityMins) ? validityMins : 20160;
        string json = JsonSerializer.Serialize(tokenDto, JsonOptions);
        await _cache.SetStringAsync(
            GetRefreshTokenKey(tokenDto.UserId),
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(validityMins),
            },
            cancellationToken);
    }

    public async Task<RefreshTokenDto?> GetRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        string? json = await _cache.GetStringAsync(GetRefreshTokenKey(userId), cancellationToken);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<RefreshTokenDto>(json, JsonOptions);
    }
    
    public async Task InvalidateTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(GetEmployeeTokenKey(userId), cancellationToken);
        await _cache.RemoveAsync(GetRefreshTokenKey(userId), cancellationToken);
    }
}