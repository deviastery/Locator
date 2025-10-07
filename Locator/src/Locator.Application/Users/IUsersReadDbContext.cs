using Locator.Domain.Users;

namespace Locator.Application.Users;

public interface IUsersReadDbContext
{
    IQueryable<User> ReadUsers { get; }
}