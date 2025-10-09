namespace Locator.Domain.Users;

public record Token
{
    public Token(string accessToken, string refreshToken, DateTime expiresAt, string tokenType = "bearer")
    {
        AccessToken = accessToken;
        TokenType = refreshToken;
        RefreshToken = tokenType;
        ExpiresAt = expiresAt;
        
    }

    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; } 
}