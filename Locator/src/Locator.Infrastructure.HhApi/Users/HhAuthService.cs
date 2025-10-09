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

    public async Task<Result<AccessTokenResponse, Error>> ExchangeCodeForTokenAsync(
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
            return Errors.TokenExchangeFailed();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var token = JsonSerializer.Deserialize<AccessTokenResponse>(json);

        if (token == null)
        {
            return Errors.InvalidTokenResponse();
        }
        return token;
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
            return Errors.UserInfoFailed();

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
        var userSessionRecord = await _usersDbContext.UserSessions
            .Include(s => s.EmployeeToken)
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);

        if (userSessionRecord == null)
            throw new InvalidOperationException("Пользователь не авторизован в HH");

        if (userSessionRecord.EmployeeToken.ExpiresAt > DateTime.UtcNow.AddMinutes(5))
            return userSessionRecord.EmployeeToken.AccessToken;

        var newEmployeeTokenResult = 
            await RefreshTokenAsync(userSessionRecord.EmployeeToken, cancellationToken);
        if (newEmployeeTokenResult.IsFailure)
        {
            throw new Exception();
        }
        var newEmployeeToken = newEmployeeTokenResult.Value;
        
        var newSessionIdResult = await _usersRepository.UpdateEmployeeTokenUserSessionAsync(
            userSessionRecord.Id,
            newEmployeeToken, 
            cancellationToken);
        if (newSessionIdResult.IsFailure)
        {
            throw new Exception();
        }
        var newSessionId = newSessionIdResult.Value;
        
        return newEmployeeToken.AccessToken;
    }
    
    private async Task<Result<Token, Error>> RefreshTokenAsync(
        Token tokenRecord, 
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.hh.ru/token");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = tokenRecord.RefreshToken,
            ["client_id"] = _config.ClientId,
            ["client_secret"] = _config.ClientSecret
        });
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return Errors.TokenExchangeFailed();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var token = JsonSerializer.Deserialize<AccessTokenResponse>(json);

        if (token == null)
        {
            return Errors.InvalidTokenResponse();
        }
        // return token;

        var expiresAt = token.ExpiresAt;
        var newExpiresAt = DateTime.UtcNow.AddSeconds(expiresAt);

        tokenRecord.RefreshToken = token.RefreshToken;
        tokenRecord.AccessToken = token.AccessToken;
        tokenRecord.ExpiresAt = newExpiresAt;
        
        return tokenRecord;
    }
}