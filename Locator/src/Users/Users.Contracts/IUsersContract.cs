using CSharpFunctionalExtensions;
using Shared;
using Users.Contracts.Dto;

namespace Users.Contracts;

public interface IUsersContract
{
    /// <summary>
    /// Gets user
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User dto</returns>
    Task<Result<UserDto, Error>> GetUserDtoAsync(Guid userId, CancellationToken cancellationToken);
        
    /// <summary>
    /// Updates token of a job search service
    /// </summary>
    /// <param name="token">Dto of Access token of a job search service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of token of a job search service</returns>
    Task<Result<Guid, Error>> UpdateEmployeeTokenAsync(
        EmployeeTokenDto token, 
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets token of a job search service by user ID
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dto of token of a job search service</returns>
    Task<Result<EmployeeTokenDto, Error>> GetEmployeeTokenDtoByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);
}