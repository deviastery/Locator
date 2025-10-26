namespace Locator.Domain.Users;

public abstract class BaseToken
{
    public BaseToken(string token, DateTime createdAt, long expiresAt)
    {
        Id = Guid.NewGuid();
        Token = token;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }
    
    public Guid Id { get; init; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; } 
    public long ExpiresAt { get; set; } 
}