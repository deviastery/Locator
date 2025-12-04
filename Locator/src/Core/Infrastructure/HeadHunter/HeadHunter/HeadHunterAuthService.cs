using System.Net.Http.Headers;
using System.Text.Json;
using CSharpFunctionalExtensions;
using HeadHunter.Contracts;
using HeadHunter.Fails;
using HeadHunter.Fails.Exceptions;
using Microsoft.Extensions.Options;
using Redis.Contracts;
using Shared;
using Shared.Fails.Exceptions;
using Shared.Options;
using Users.Contracts;
using Users.Contracts.Dto;
using Users.Contracts.Responses;

namespace HeadHunter;

public class HeadHunterAuthService : IAuthContract
{
    private readonly HttpClient _httpClient;
    private readonly HeadHunterOptions _config;
    private readonly IUsersContract _usersContract;
    private readonly ITokenCacheContract _tokenCache;

    public HeadHunterAuthService(
        HttpClient httpClient, 
        IOptions<HeadHunterOptions> config, 
        IUsersContract usersContract,
        ITokenCacheContract tokenCache)
    {
        _httpClient = httpClient;
        _config = config.Value; 
        _usersContract = usersContract;
        _tokenCache = tokenCache;
    }

    public string GetAuthorizationUrl()
    {
        if (string.IsNullOrWhiteSpace(_config.ClientId) ||
            string.IsNullOrWhiteSpace(_config.RedirectUri) ||
            string.IsNullOrWhiteSpace(_config.Scope))
        {
            throw new ConfigurationFailureException("Failed to get hh api options.");
        }
        
        string url = $"https://hh.ru/oauth/authorize?" +
                     $"response_type=code&" +
                     $"client_id={Uri.EscapeDataString(_config.ClientId)}&" +
                     $"redirect_uri={Uri.EscapeDataString(_config.RedirectUri)}&" +
                     $"scope={Uri.EscapeDataString(_config.Scope)}";

        return url;
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
        var tokenDto = await _usersContract
            .GetEmployeeTokenDtoByUserIdAsync(userId, cancellationToken);
        if (tokenDto is null)
        {
            return Errors.General.NotFound("Employee token not found");
        }

        if (tokenDto == null)
        {
            throw new UserUnauthorizedException();
        }

        // Check expired time of Employee access token
        DateTime createdDateTime = tokenDto.CreatedAt;
        DateTime expiredDateTime = createdDateTime.AddSeconds(tokenDto.ExpiresAt);

        // Return Employee access token, if it has not expired
        if (expiredDateTime.ToUniversalTime() > DateTime.UtcNow)
        {
            return tokenDto.Token;
        }

        // Get a new Employee token, if it has expired
        (_, bool isFailure, EmployeeTokenDto? newEmployeeToken, Error? error) = 
            await RefreshTokenAsync(tokenDto, cancellationToken);
        if (isFailure)
        {
            return error;
        }

        // Update Employee token
        var newEmployeeTokenIdResult = await _usersContract.UpdateEmployeeTokenAsync(
            newEmployeeToken, 
            cancellationToken);
        if (newEmployeeTokenIdResult.IsFailure)
        {
            return newEmployeeTokenIdResult.Error;
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
    
    private async Task<Result<EmployeeTokenDto, Error>> RefreshTokenAsync(
        EmployeeTokenDto tokenRecord, 
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