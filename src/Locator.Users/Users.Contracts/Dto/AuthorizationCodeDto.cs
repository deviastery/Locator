namespace Users.Contracts.Dto;

public record AuthorizationCodeDto(
    string Code,
    string? State
    );