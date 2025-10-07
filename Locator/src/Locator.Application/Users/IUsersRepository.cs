using CSharpFunctionalExtensions;
using Locator.Domain.Users;
using Shared;

namespace Locator.Application.Users;

public interface IUsersRepository
{
    Task<Guid> CreateAsync(User user, CancellationToken cancellationToken);
}