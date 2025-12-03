using Users.Contracts.Dto;

namespace Redis.Contracts;

public interface ITokenCacheContract
{
    /// <summary>
    /// Sets employee access token a job search service
    /// </summary>
    /// <param name="token">Access token of a job search service</param>
    /// <param name="userId">ID of user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="expiry">Expiration time</param>
    /// <returns></returns>
    Task SetEmployeeTokenAsync(
        string token, 
        Guid userId,
        CancellationToken cancellationToken,
        TimeSpan? expiry = null);
    
    /// <summary>
    /// Gets employee access token a job search service
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token of a job search service</returns>
    Task<string?> GetEmployeeTokenAsync(
        Guid userId,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Sets refresh access token a job search service
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="expiry">Expiration time</param>
    /// <returns></returns>
    Task SetRefreshTokenAsync(
        RefreshTokenDto token,
        CancellationToken cancellationToken,
        TimeSpan? expiry = null);
    
    /// <summary>
    /// Gets employee access token a job search service
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token</returns>
    Task<RefreshTokenDto?> GetRefreshTokenAsync(
        Guid userId,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Invalidate tokens
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task InvalidateTokensAsync(
        Guid userId,
        CancellationToken cancellationToken);
}