using CSharpFunctionalExtensions;
using HeadHunter.Contracts.Dto;
using HeadHunter.Contracts.Responses;
using Shared;

namespace HeadHunter.Contracts;

public interface IAuthContract
{
    /// <summary>
    /// Gets authorization url
    /// </summary>
    /// <returns>Url authorization string</returns>
    string GetAuthorizationUrl();
    
    /// <summary>
    /// Exchanges authorization code for access and refresh tokens
    /// </summary>
    /// <param name="code">Authorization code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token of a job search service & Time that token created at</returns>
    Task<Result<(AccessTokenResponse tokenResponse, DateTime createdAt), Error>> ExchangeCodeForTokenAsync(
        string code, 
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets user info from a job search service
    /// </summary>
    /// <param name="accessToken">Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User info of a job search service</returns>
    Task<Result<UserDto, Error>> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a new access token of a job search service
    /// </summary>
    /// <param name="tokenRecord">Old access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access token of a job search service</returns>
    Task<Result<EmployeeTokenDto, Error>> RefreshTokenAsync(
        EmployeeTokenDto tokenRecord,
        CancellationToken cancellationToken);
}