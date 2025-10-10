using CSharpFunctionalExtensions;
using Locator.Domain.Users;
using Shared;

namespace Locator.Application.Users;

public interface IUsersRepository
{
    Task<Guid> CreateAsync(User user, CancellationToken cancellationToken);
    Task<Result<Guid, Error>> UpdateEmployeeSessionAsync(EmployeeToken token, CancellationToken cancellationToken);
    Task<Result<Guid, Error>> CreateRefreshTokenAsync( RefreshToken? token, CancellationToken cancellationToken);
    Task DeleteRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenAsync(string token);
    User GetUserAsync(Guid userId);
}