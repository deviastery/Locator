using CSharpFunctionalExtensions;
using Locator.Domain.Users;
using Shared;

namespace Locator.Application.Users;

public interface IUsersRepository
{
    Task<Result<Guid, Error>> CreateUserAsync(User user, CancellationToken cancellationToken);
    Task<Result<User, Error>> GetUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<Guid, Error>> UpdateEmployeeTokenAsync(EmployeeToken token, CancellationToken cancellationToken);
    Task<Result<Guid, Error>> CreateEmployeeTokenAsync(EmployeeToken token, CancellationToken cancellationToken);
    Task<Result<string, Error>> CreateRefreshTokenAsync(RefreshToken? token, CancellationToken cancellationToken);
    Task<Result<string, Error>> DeleteRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<Result<RefreshToken, Error>> GetRefreshTokenAsync(string token, CancellationToken cancellationToken);
}