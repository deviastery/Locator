namespace Locator.Contracts.Users;

public record AuthorizationCodeDto(
    string Code,
    string? State
    );