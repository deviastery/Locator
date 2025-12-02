namespace Users.Contracts.Dto;

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
    
    public Guid Id { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; }
    public long ExpiresAt { get; set; }
    public Guid UserId { get; set; }
}