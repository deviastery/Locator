using System.Text.Json.Serialization;

namespace Users.Contracts.Responses;

public record AccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = default!;

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "bearer";

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = default!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}