namespace Users.Contracts.Responses;

public record AuthResponse(Guid? UserId, string? AccessToken, int ExpiresIn, string? RefreshToken);