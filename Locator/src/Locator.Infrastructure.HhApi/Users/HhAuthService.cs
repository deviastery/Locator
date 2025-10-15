using System.Net.Http.Headers;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Locator.Application.Users;
using Locator.Application.Users.Fails;
using Locator.Contracts.Users;
using Locator.Domain.Users;
using Locator.Infrastructure.Postgresql.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;

namespace Locator.Infrastructure.HhApi.Users;

public class HhAuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly HhApiConfiguration _config;
    private readonly UsersDbContext _usersDbContext;
    private readonly IUsersRepository _usersRepository;

    public HhAuthService(
        HttpClient httpClient, 
        IOptions<HhApiConfiguration> config, 
        UsersDbContext usersDbContext,
        IUsersRepository usersRepository)
    {
        _httpClient = httpClient;
        _config = config.Value; 
        _usersDbContext = usersDbContext;
        _usersRepository = usersRepository;
    }

    public async Task<Result<(AccessTokenResponse tokenResponse, DateTime createdAt), Error>> ExchangeCodeForTokenAsync(
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
            ["grant_type"] = "authorization_code"
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
            return Errors.GetUserInfoFailed();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var user = JsonSerializer.Deserialize<UserDto>(json);
        return user?.Email != null
            ? user
            : Errors.MissingEmail();
    }

    public async Task<Result<string, Error>> GetValidEmployeeAccessTokenAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        var tokenRecord = await _usersDbContext.ReadEmployeeTokens
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);

        if (tokenRecord == null)
            throw new InvalidOperationException("Пользователь не авторизован в HH");

        DateTime createdDateTime = tokenRecord.CreatedAt;
        DateTime expiredDateTime = createdDateTime.AddSeconds(tokenRecord.ExpiresIn);
        
        if (expiredDateTime.ToUniversalTime() > DateTime.UtcNow)
            return tokenRecord.AccessToken;

        var newEmployeeTokenResult = 
            await GetRefreshTokenAsync(tokenRecord, cancellationToken);
        if (newEmployeeTokenResult.IsFailure)
        {
            throw new Exception();
        }
        var newEmployeeToken = newEmployeeTokenResult.Value;
        
        var newEmployeeTokensIdResult = await _usersRepository.UpdateEmployeeTokenAsync(
            newEmployeeToken, 
            cancellationToken);
        if (newEmployeeTokensIdResult.IsFailure)
        {
            throw new Exception();
        }
        
        return newEmployeeToken.AccessToken;
    }
    
    private async Task<Result<EmployeeToken, Error>> GetRefreshTokenAsync(
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

        tokenRecord.RefreshToken = token.RefreshToken;
        tokenRecord.AccessToken = token.AccessToken;
        tokenRecord.CreatedAt = DateTime.UtcNow;
        tokenRecord.ExpiresIn = token.ExpiresIn;
        
        return tokenRecord;
    }
}