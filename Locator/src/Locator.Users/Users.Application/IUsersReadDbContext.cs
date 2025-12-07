using Users.Domain;

namespace Users.Application;

public interface IUsersReadDbContext
{
    IQueryable<User> ReadUsers { get; }
    IQueryable<RefreshToken> ReadRefreshTokens { get; }
    IQueryable<EmployeeToken> ReadEmployeeTokens { get; }
}