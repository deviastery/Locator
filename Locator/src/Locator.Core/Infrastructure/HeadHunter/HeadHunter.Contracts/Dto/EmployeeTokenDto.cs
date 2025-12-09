using System.Text.Json.Serialization;

namespace HeadHunter.Contracts.Dto;

public record EmployeeTokenDto
{
    public EmployeeTokenDto(
        Guid id,
        string token,
        string refreshToken,
        DateTime createdAt,
        long expiresAt,
        Guid userId)
    {
        Id = id;
        Token = token;
        RefreshToken = refreshToken;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        UserId = userId;
    }
    
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("token")]
    public string Token { get; set; }
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("expiresAt")]
    public long ExpiresAt { get; set; }
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }
}