namespace Locator.Domain.Users;

public abstract class BaseToken
{
    public BaseToken(string token, DateTime createdAt, long expiresIn)
    {
        Id = Guid.NewGuid();
        Token = token;
        CreatedAt = createdAt;
        ExpiresIn = expiresIn;
    }
    
    public Guid Id { get; init; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; } 
    public long ExpiresIn { get; set; } 
}