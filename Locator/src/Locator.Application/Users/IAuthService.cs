using CSharpFunctionalExtensions;
using Locator.Contracts.Users.Dtos;
using Locator.Contracts.Users.Responses;
using Shared;

namespace Locator.Application.Users;

public interface IAuthService
{
    /// <summary>
    /// Method for exchange authorization code for access and refresh tokens
    /// </summary>
    /// <param name="code">Authorization code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token of a job search service & Time that token created at</returns>
    Task<Result<(AccessTokenResponse tokenResponse, DateTime createdAt), Error>> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method for getting user info from a job search service
    /// </summary>
    /// <param name="accessToken">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User info of a job search service</returns>
    Task<Result<UserDto, Error>> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method for getting valid access token of a job search service
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token of a job search service</returns>
    Task<Result<string, Error>> GetValidEmployeeAccessTokenAsync(Guid userId, CancellationToken cancellationToken);
}