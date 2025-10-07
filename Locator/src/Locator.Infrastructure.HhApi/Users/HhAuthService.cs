using System.Net.Http.Headers;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Locator.Application.Users;
using Locator.Application.Users.Fails;
using Locator.Contracts.Users;
using Microsoft.Extensions.Options;
using Shared;

namespace Locator.Infrastructure.HhApi.Users;

public class HhAuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly HhApiConfiguration _config;

    public HhAuthService(
        HttpClient httpClient, 
        IOptions<HhApiConfiguration> config)
    {
        _httpClient = httpClient;
        _config = config.Value; 
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
}