namespace Locator.Domain.Users;

public record Token
{
    public Token(string accessToken, string refreshToken, DateTime createdAt, long expiresIn, string tokenType = "bearer")
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        RefreshToken = refreshToken;
        CreatedAt = createdAt;
        ExpiresIn = expiresIn;
    }

    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public string RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; } 
    public long ExpiresIn { get; set; } 
}