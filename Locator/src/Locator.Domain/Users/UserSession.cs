namespace Locator.Domain.Users;

public record UserSession
{
    public UserSession(Guid userId, Token employeeToken, Token token)
    {
        UserId = userId;
        EmployeeToken = employeeToken;
        Token = token;
    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Token EmployeeToken { get; set; }
    public Token Token { get; set; }
};