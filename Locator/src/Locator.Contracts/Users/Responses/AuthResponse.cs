namespace Locator.Contracts.Users.Responses;

public record AuthResponse(string? UserName, string? AccessToken, int ExpiresIn, string? RefreshToken);