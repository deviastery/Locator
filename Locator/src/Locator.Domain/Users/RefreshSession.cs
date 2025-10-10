namespace Locator.Domain.Users;

public record RefreshSession
{
    public RefreshSession(
        Guid userId, 
        string accessToken, 
        string refreshToken, 
        string fingerprint, 
        string ip, 
        long expiresIn, 
        DateTime createdAt, 
        string userAgent = "Locator/1.0")
    {
        UserId = userId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        Fingerprint = fingerprint;
        Ip = ip;
        ExpiresIn = expiresIn;
        CreatedAt = createdAt;
        UserAgent = userAgent;
    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string UserAgent { get; set; }
    public string Fingerprint { get; set; }
    public string Ip { get; set; }
    public long ExpiresIn { get; set; }
    public DateTime CreatedAt { get; set; }
};