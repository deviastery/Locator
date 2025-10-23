using CSharpFunctionalExtensions;
using Locator.Domain.Users;
using Shared;

namespace Locator.Application.Users;

public interface IUsersRepository
{
    /// <summary>
    /// Method for creating user
    /// </summary>
    /// <param name="user">User</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of a new user</returns>
    Task<Result<Guid, Error>> CreateUserAsync(User user, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method for getting user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>User</returns>
    Task<Result<User, Error>> GetUserAsync(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method for updating token of a job search service
    /// </summary>
    /// <param name="token">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of token of a job search service</returns>
    Task<Result<Guid, Error>> UpdateEmployeeTokenAsync(EmployeeToken token, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method for creating token of a job search service
    /// </summary>
    /// <param name="token">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of new token of a job search service</returns>
    Task<Result<Guid, Error>> CreateEmployeeTokenAsync(EmployeeToken token, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method for creating refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token</returns>
    Task<Result<string, Error>> CreateRefreshTokenAsync(RefreshToken? token, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method for deleting refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deleted refresh token</returns>
    Task<Result<string, Error>> DeleteRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method for creating refresh token
    /// </summary>
    /// <param name="token">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token</returns>
    Task<Result<RefreshToken, Error>> GetRefreshTokenAsync(string token, CancellationToken cancellationToken);
}