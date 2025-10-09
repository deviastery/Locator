using CSharpFunctionalExtensions;
using Shared;

namespace Locator.Application.Users;

public interface ITokenService
{
    string GenerateToken(Guid userId, string email);
    Task<Result<string, Error>> GetValidAccessTokenAsync(Guid userId, CancellationToken cancellationToken);
}