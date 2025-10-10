namespace Locator.Contracts.Users;

public record AuthResponse(string? userName, string? AccessToken, int ExpiresIn, string? RefreshToken);