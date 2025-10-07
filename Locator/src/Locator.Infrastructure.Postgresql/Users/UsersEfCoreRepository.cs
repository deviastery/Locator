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

    public async Task<Result<User?, Error>> GetByHhIdAsync(long hhId, CancellationToken cancellationToken)
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
}