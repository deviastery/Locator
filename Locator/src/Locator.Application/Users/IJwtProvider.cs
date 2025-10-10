using CSharpFunctionalExtensions;
using Locator.Domain.Users;
using Shared;

namespace Locator.Application.Users;

public interface IJwtProvider
{
    (string, int) GenerateJwtToken(User user);
    Task<string?> GenerateRefreshToken(Guid userId, CancellationToken cancellationToken);
    Task<(string, int)?> ValidateRefreshToken(string token);
}