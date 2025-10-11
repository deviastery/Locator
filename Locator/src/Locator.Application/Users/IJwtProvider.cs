using CSharpFunctionalExtensions;
using Locator.Domain.Users;
using Shared;

namespace Locator.Application.Users;

public interface IJwtProvider
{
    (string Token, int ExpiresIn) GenerateJwtToken(User user);
    Task<string?> GenerateRefreshToken(Guid userId, CancellationToken cancellationToken);
    Task<(string Token, int ExpiresIn)?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken);
}