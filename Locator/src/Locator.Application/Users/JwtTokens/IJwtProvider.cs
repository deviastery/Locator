using CSharpFunctionalExtensions;
using Locator.Domain.Users;
using Shared;

namespace Locator.Application.Users.JwtTokens;

public interface IJwtProvider
{
    /// <summary>
    /// Generates JWT access token
    /// </summary>
    /// <param name="user">User</param>
    /// <returns>Access token & Time of token expiration</returns>
    (string Token, int ExpiresIn) GenerateJwtToken(User user);
    
    /// <summary>
    /// Generates refresh token
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token</returns>
    Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Refreshes access token
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token</returns>
    Task<Result<string, Error>> RefreshAccessTokenAsync(Guid userId, CancellationToken cancellationToken);
}