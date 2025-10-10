namespace Locator.Domain.Users;

public record RefreshToken(string? Token, DateTime ExpiresIn, Guid UserId);