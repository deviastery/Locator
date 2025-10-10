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

    public async Task<Result<Guid, Error>> UpdateEmployeeSessionAsync(
        EmployeeToken token,
        CancellationToken cancellationToken)
    {
        try
        {
            var newToken = await _usersDbContext.EmployeeTokens
                .SingleOrDefaultAsync(s => s.Id == token.Id, cancellationToken);

            if (newToken == null)
            {
                return Errors.General.NotFound(token.Id);
            }
            
            newToken.RefreshToken = token.RefreshToken;
            newToken.AccessToken = token.AccessToken;
            newToken.CreatedAt = DateTime.UtcNow;
            newToken.ExpiresIn = token.ExpiresIn;
            
            await _usersDbContext.SaveChangesAsync(cancellationToken);
            return newToken.Id;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }

    public Task<Result<Guid, Error>> CreateRefreshTokenAsync(RefreshToken? token, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task DeleteRefreshTokenAsync(RefreshToken refreshToken) => throw new NotImplementedException();

    public Task<RefreshToken> GetRefreshTokenAsync(string token) => throw new NotImplementedException();

    public User GetUserAsync(Guid userId) => throw new NotImplementedException();
}