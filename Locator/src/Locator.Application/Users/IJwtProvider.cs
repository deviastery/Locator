using CSharpFunctionalExtensions;
using Locator.Domain.Users;
using Shared;

namespace Locator.Application.Users;

public interface IJwtProvider
{
    (string Token, int ExpiresIn) GenerateJwtToken(User user);
    Task<string?> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    Task<string> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken);
}