namespace Locator.Domain.Users;

public record EmployeeToken : Token
{
    public EmployeeToken(
        Guid userId, 
        string accessToken, 
        string refreshToken, 
        DateTime createdAt, 
        long expiresIn, 
        string tokenType = "bearer")
    : base(accessToken, refreshToken, createdAt, expiresIn, tokenType)
    {
        Id = Guid.NewGuid();
        UserId = userId;
    }
    
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}