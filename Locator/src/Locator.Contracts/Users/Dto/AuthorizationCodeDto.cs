namespace Locator.Contracts.Users.Dto;

public record AuthorizationCodeDto(
    string Code,
    string? State
    );