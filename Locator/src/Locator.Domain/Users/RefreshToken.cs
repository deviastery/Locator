namespace Locator.Domain.Users;

public class RefreshToken : BaseToken
{
    public RefreshToken(
        string token, 
        DateTime createdAt, 
        long expiresIn,
        Guid userId)
        : base(token, createdAt, expiresIn)
    {
        UserId = userId;
    }
    
    public Guid UserId { get; set; }
}