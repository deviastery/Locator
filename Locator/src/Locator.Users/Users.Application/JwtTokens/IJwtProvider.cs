using CSharpFunctionalExtensions;
using Shared;
using Users.Domain;

namespace Users.Application.JwtTokens;

public interface IJwtProvider
{
    /// <summary>
    /// Generates JWT access token
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="email">Email of user</param>
    /// <returns>Access token & Time of token expiration</returns>
    (string Token, int ExpiresIn) GenerateJwtToken(Guid userId, string? email);
    
    /// <summary>
    /// Generates refresh token
    /// </summary>
    /// <param name="userId">ID of user</param>
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

    /// <summary>
    /// Deletes refresh tokens from DB 
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task DeleteRefreshTokensByUserId(Guid userId, CancellationToken cancellationToken);
}