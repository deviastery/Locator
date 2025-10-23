namespace Locator.Contracts.Users.Dtos;

public record AuthorizationCodeDto(
    string Code,
    string? State
    );