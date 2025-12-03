namespace Users.Domain;

public class EmployeeToken : BaseToken
{
    public EmployeeToken(
        string token, 
        string refreshToken, 
        DateTime createdAt, 
        long expiresAt, 
        Guid userId, 
        string tokenType = "bearer")
    : base(token, createdAt, expiresAt)
    {
        RefreshToken = refreshToken;
        UserId = userId;
        TokenType = tokenType;
    }
    
    public string TokenType { get; set; }
    public string RefreshToken { get; set; }
    public Guid UserId { get; set; }
}