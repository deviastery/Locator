namespace Users.Domain;

public class RefreshToken : BaseToken
{
    public RefreshToken(
        string token, 
        DateTime createdAt, 
        long expiresAt,
        Guid userId)
        : base(token, createdAt, expiresAt)
    {
        UserId = userId;
    }
    
    public Guid UserId { get; set; }
}