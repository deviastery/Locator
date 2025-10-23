using CSharpFunctionalExtensions;
using Locator.Domain.Users;
using Shared;

namespace Locator.Application.Users;

public interface IJwtProvider
{
    /// <summary>
    /// Method for generating JWT access token
    /// </summary>
    /// <param name="user">User</param>
    /// <returns>Access token & Time of token expiration</returns>
    (string Token, int ExpiresIn) GenerateJwtToken(User user);
    
    /// <summary>
    /// Method for generating refresh token
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token</returns>
    Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method for refreshing access token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token</returns>
    Task<string> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken);
}