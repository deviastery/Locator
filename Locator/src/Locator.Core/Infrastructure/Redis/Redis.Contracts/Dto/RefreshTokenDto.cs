namespace Redis.Contracts.Dto;

public record RefreshTokenDto(
    Guid Id,
    string Token, 
    DateTime CreatedAt, 
    long ExpiresAt,
    Guid UserId);