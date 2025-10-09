using CSharpFunctionalExtensions;
using Locator.Application.Ratings.Fails;
using Locator.Application.Users;
using Locator.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Locator.Infrastructure.Postgresql.Users;

public class UsersEfCoreRepository : IUsersRepository
{
    private UsersDbContext _usersDbContext;

    public UsersEfCoreRepository(UsersDbContext usersDbContext)
    {
        _usersDbContext = usersDbContext;
    }

    public async Task<Result<User?, Error>> GetByEmployeeIdAsync(long hhId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _usersDbContext.Users
                .SingleOrDefaultAsync(u => u.EmployeeId == hhId, cancellationToken);

            if (user == null)
            {
                return Errors.General.NotFound(hhId);
            }
            return user;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }

    public async Task<Guid> CreateAsync(User user, CancellationToken cancellationToken)
    {
        await _usersDbContext.Users.AddAsync(user, cancellationToken);
        await _usersDbContext.SaveChangesAsync(cancellationToken);
        return user.Id;
    }

    public async Task<Result<Guid, Error>> UpdateEmployeeTokenUserSessionAsync(
        Guid sessionId,
        Token employeeToken, 
        CancellationToken cancellationToken)
    {
        try
        {
            var session = await _usersDbContext.UserSessions
                .SingleOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

            if (session == null)
            {
                return Errors.General.NotFound(sessionId);
            }
            session.EmployeeToken = employeeToken;
            await _usersDbContext.SaveChangesAsync(cancellationToken);
            return session.Id;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
}