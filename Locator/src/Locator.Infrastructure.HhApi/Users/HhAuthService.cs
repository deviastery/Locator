using System.Net.Http.Headers;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Locator.Application.Users;
using Locator.Contracts.Users.Dto;
using Locator.Contracts.Users.Responses;
using Locator.Domain.Users;
using Locator.Infrastructure.HhApi.Users.Fails.Exceptions;
using Locator.Infrastructure.Postgresql.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;
using Errors = Locator.Infrastructure.HhApi.Users.Fails.Errors;

namespace Locator.Infrastructure.HhApi.Users;

public class HhAuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly HhApiConfiguration _config;
    private readonly UsersDbContext _usersDbContext;
    private readonly IUsersRepository _usersRepository;
    private readonly ITokenCacheService _tokenCache;

    public HhAuthService(
        HttpClient httpClient, 
        IOptions<HhApiConfiguration> config, 
        UsersDbContext usersDbContext,
        IUsersRepository usersRepository,
        ITokenCacheService tokenCache)
    {
        _httpClient = httpClient;
        _config = config.Value; 
        _usersDbContext = usersDbContext;
        _usersRepository = usersRepository;
        _tokenCache = tokenCache;
    }

    public async Task<Result<(
        AccessTokenResponse tokenResponse, 
        DateTime createdAt), Error>> ExchangeCodeForTokenAsync(
        string code, 
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.hh.ru/token");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _config.ClientId,
            ["client_secret"] = _config.ClientSecret,
            ["code"] = code,
            ["redirect_uri"] = _config.RedirectUri,
            ["grant_type"] = "authorization_code",
        });

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Errors.TokenExchangeFailed();
        }

        var createdAt = DateTime.UtcNow;

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var token = JsonSerializer.Deserialize<AccessTokenResponse>(json);

        if (token == null)
        {
            return Errors.InvalidTokenResponse();
        }
        
        return (token, createdAt);
    }

    public async Task<Result<UserDto, Error>> GetUserInfoAsync(
        string accessToken, 
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.hh.ru/me");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Errors.General.Failure("Get user info failed");
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var user = JsonSerializer.Deserialize<UserDto>(json);
        return user?.Email != null
            ? user
            : Errors.General.NotFound("Email address not found");
    }

    public async Task<Result<string, Error>> GetValidEmployeeAccessTokenAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Get Employee token
        var tokenRecord = await _usersDbContext.ReadEmployeeTokens
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);

        if (tokenRecord == null)
        {
            throw new UserUnauthorizedException();
        }

        // Check expired time of Employee access token
        DateTime createdDateTime = tokenRecord.CreatedAt;
        DateTime expiredDateTime = createdDateTime.AddSeconds(tokenRecord.ExpiresAt);

        // Return Employee access token, if it has not expired
        if (expiredDateTime.ToUniversalTime() > DateTime.UtcNow)
        {
            return tokenRecord.Token;
        }

        // Get a new Employee token, if it has expired
        (_, bool isFailure, EmployeeToken? newEmployeeToken, Error? error) = 
            await RefreshTokenAsync(tokenRecord, cancellationToken);
        if (isFailure)
        {
            return error;
        }

        // Update Employee token
        var newEmployeeTokensIdResult = await _usersRepository.UpdateEmployeeTokenAsync(
            newEmployeeToken, 
            cancellationToken);
        if (newEmployeeTokensIdResult.IsFailure)
        {
            return newEmployeeTokensIdResult.Error;
        }
        
        return newEmployeeToken.Token;
    }
    
    public async Task<string?> GetEmployeeTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            // Try to get Employee token from cache
            string? cachedEmployeeToken = await _tokenCache.GetEmployeeTokenAsync(userId, cancellationToken);
            if (cachedEmployeeToken != null)
            {
                return cachedEmployeeToken;
            }

            // If Cache miss try to get Employee token from DB
            (_, bool isFailure, string? dbEmployeeToken) =
                await GetValidEmployeeAccessTokenAsync(userId, cancellationToken);
            if (isFailure)
            {
                return null;
            }

            // Set Employee token in cache
            if (dbEmployeeToken != null)
            {
                await _tokenCache.SetEmployeeTokenAsync(dbEmployeeToken, userId, cancellationToken);
            }

            return dbEmployeeToken;
        }
        catch
        {
            return null;
        }
    }
    
    private async Task<Result<EmployeeToken, Error>> RefreshTokenAsync(
        EmployeeToken tokenRecord, 
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.hh.ru/token");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = tokenRecord.RefreshToken,
            ["client_id"] = _config.ClientId,
            ["client_secret"] = _config.ClientSecret,
        });
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Errors.TokenExchangeFailed();
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var token = JsonSerializer.Deserialize<AccessTokenResponse>(json);

        if (token == null)
        {
            return Errors.InvalidTokenResponse();
        }

        // Update Access token
        tokenRecord.RefreshToken = token.RefreshToken;
        tokenRecord.Token = token.AccessToken;
        tokenRecord.CreatedAt = DateTime.UtcNow;
        tokenRecord.ExpiresAt = token.ExpiresIn;
        
        return tokenRecord;
    }
}