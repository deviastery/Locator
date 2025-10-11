namespace Locator.Domain.Users;

public record RefreshToken(Guid Token, DateTime ExpiresIn, Guid UserId);