using CSharpFunctionalExtensions;
using Locator.Application.Ratings.Fails;
using Locator.Application.Users;
using Locator.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Locator.Infrastructure.Postgresql.Users;

public class UsersEfCoreRepository : IUsersRepository
{
    private readonly UsersDbContext _usersDbContext;

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

    public async Task<Result<Guid, Error>> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            var newUser = await _usersDbContext.Users.AddAsync(user, cancellationToken);
            await _usersDbContext.SaveChangesAsync(cancellationToken);
            return newUser.Entity.Id;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }

    public async Task<Result<User, Error>> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var userRecord = await _usersDbContext.Users
                .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (userRecord == null)
            {
                return Errors.General.NotFound(userId);
            }
            return userRecord;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }

    public async Task<Result<Guid, Error>> UpdateEmployeeTokensAsync(
        EmployeeToken token,
        CancellationToken cancellationToken)
    {
        try
        {
            var newToken = await _usersDbContext.EmployeeTokens
                .SingleOrDefaultAsync(t => t.Id == token.Id, cancellationToken);

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

    public async Task<Result<string, Error>> CreateRefreshTokenAsync(RefreshToken? token, CancellationToken cancellationToken)
    {
        try
        {
            if (token is null)
            {
                return Errors.General.Failure("Token is empty");
            }
            var newToken = await _usersDbContext.RefreshTokens.AddAsync(token, cancellationToken);
            await _usersDbContext.SaveChangesAsync(cancellationToken);
            return newToken.Entity.Token.ToString();
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }

    public async Task<Result<string, Error>> DeleteRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        try
        {
            var deletedToken = await _usersDbContext.RefreshTokens
                .SingleOrDefaultAsync(t => t.Token == token.Token, cancellationToken);

            if (deletedToken == null)
            {
                return Errors.General.NotFound(token.Token);
            }

            _usersDbContext.Remove(deletedToken);
            await _usersDbContext.SaveChangesAsync(cancellationToken);
            return token.Token.ToString();
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }

    public async Task<Result<RefreshToken, Error>> GetRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            var tokenRecord = await _usersDbContext.RefreshTokens
                .SingleOrDefaultAsync(t => t.Token.ToString() == token, cancellationToken);

            if (tokenRecord == null)
            {
                return Errors.General.NotFound(token);
            }
            return tokenRecord;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
}