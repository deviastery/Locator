using CSharpFunctionalExtensions;
using Locator.Domain.Users;
using Shared;

namespace Locator.Application.Users;

public interface IUsersRepository
{
    /// <summary>
    /// Creates user
    /// </summary>
    /// <param name="user">User</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of a new user</returns>
    Task<Result<Guid, Error>> CreateUserAsync(User user, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets user
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User</returns>
    Task<Result<User, Error>> GetUserAsync(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Updates token of a job search service
    /// </summary>
    /// <param name="token">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of token of a job search service</returns>
    Task<Result<Guid, Error>> UpdateEmployeeTokenAsync(EmployeeToken token, CancellationToken cancellationToken);
    
    /// <summary>
    /// Creates token of a job search service
    /// </summary>
    /// <param name="token">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of new token of a job search service</returns>
    Task<Result<Guid, Error>> CreateEmployeeTokenAsync(EmployeeToken token, CancellationToken cancellationToken);
    
    /// <summary>
    /// Creates refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New refresh token</returns>
    Task<Result<string, Error>> CreateRefreshTokenAsync(RefreshToken? token, CancellationToken cancellationToken);
    
    /// <summary>
    /// Deletes refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deleted refresh token</returns>
    Task<Result<string, Error>> DeleteRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets refresh token
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Full refresh token</returns>
    Task<Result<RefreshToken, Error>> GetRefreshTokenByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}