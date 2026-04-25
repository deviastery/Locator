using System.Net.Http.Headers;
using System.Text.Json;
using CSharpFunctionalExtensions;
using HeadHunter.Contracts;
using HeadHunter.Contracts.Dto;
using HeadHunter.Contracts.Responses;
using HeadHunter.Fails;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Fails.Exceptions;
using Shared.Options;

namespace HeadHunter;

public class HeadHunterAuthService : IAuthContract
{
    private readonly HttpClient _httpClient;
    private readonly HeadHunterOptions _config;

    public HeadHunterAuthService(
        HttpClient httpClient, 
        IOptions<HeadHunterOptions> config)
    {
        _httpClient = httpClient;
        _config = config.Value; 
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
    
    public async Task<Result<EmployeeTokenDto, Error>> RefreshTokenAsync(
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