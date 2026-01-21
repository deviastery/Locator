using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Shared;
using Users.Application;
using Users.Application.Fails;
using Users.Domain;

namespace Users.Infrastructure.Postgresql;

public class UsersEfCoreRepository : IUsersRepository
{
    private readonly UsersDbContext _usersDbContext;

    public UsersEfCoreRepository(UsersDbContext usersDbContext)
    {
        _usersDbContext = usersDbContext;
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
                return Errors.General.NotFound($"User not found be ID={userId}");
            }
            return userRecord;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
    
    public async Task<Result<Guid, Error>> CreateEmployeeTokenAsync(
        EmployeeToken? token, 
        CancellationToken cancellationToken)
    {
        try
        {
            if (token is null)
            {
                return Errors.General.Failure("Token is empty");
            }
            var newToken = await _usersDbContext.EmployeeTokens
                .AddAsync(token, cancellationToken);
            await _usersDbContext.SaveChangesAsync(cancellationToken);
            return newToken.Entity.Id;
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
            return newToken.Entity.Token;
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
                return Errors.General.NotFound($"Token not found by ID={token.Id}");
            }

            _usersDbContext.Remove(deletedToken);
            await _usersDbContext.SaveChangesAsync(cancellationToken);
            return token.Token;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }

    public async Task<Result<RefreshToken, Error>> GetRefreshTokenByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var tokenRecord = await _usersDbContext.RefreshTokens
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (tokenRecord == null)
            {
                return Errors.RefreshTokenByUserIdNotFound();
            }
            return tokenRecord;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
    public async Task<Result<EmployeeToken, Error>> GetEmployeeTokenByIdAsync(
        Guid tokenId, CancellationToken cancellationToken)
    {
        try
        {
            var tokenRecord = await _usersDbContext.EmployeeTokens
                .Where(t => t.Id == tokenId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (tokenRecord == null)
            {
                return Errors.RefreshTokenByUserIdNotFound();
            }
            return tokenRecord;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
    
    public async Task<Result<Guid, Error>> UpdateEmployeeTokenAsync(
        EmployeeToken newToken,
        CancellationToken cancellationToken)
    {
        try
        {
            var token = await _usersDbContext.EmployeeTokens
                .SingleOrDefaultAsync(t => t.Id == newToken.Id, cancellationToken);

            if (token == null)
            {
                return Errors.General.NotFound($"Token not found by ID={newToken.Id}");
            }
            
            token.RefreshToken = newToken.RefreshToken;
            token.Token = newToken.Token;
            token.CreatedAt = DateTime.UtcNow;
            token.ExpiresAt = newToken.ExpiresAt;
            
            await _usersDbContext.SaveChangesAsync(cancellationToken);
            return newToken.Id;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
    
    public async Task<UnitResult<Error>> DeleteRefreshTokensByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var deletedTokens = await _usersDbContext.RefreshTokens
                .Where(t => t.UserId == userId)
                .ToListAsync(cancellationToken);

            if (deletedTokens.Count == 0)
            {
                return Errors.General.NotFound($"Tokens not found by UserID={userId}");
            }

            _usersDbContext.RemoveRange(deletedTokens);
            await _usersDbContext.SaveChangesAsync(cancellationToken);
            
            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
    
    public async Task<UnitResult<Error>> DeleteEmployeeTokensByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var deletedTokens = await _usersDbContext.EmployeeTokens
                .Where(t => t.UserId == userId)
                .ToListAsync(cancellationToken);

            if (deletedTokens.Count == 0)
            {
                return Errors.General.NotFound($"Tokens not found by UserID={userId}");
            }

            _usersDbContext.RemoveRange(deletedTokens);
            await _usersDbContext.SaveChangesAsync(cancellationToken);
            
            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
    
    public async Task<Result<EmployeeToken, Error>> GetEmployeeTokenByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        try
        {
            var tokenRecord = await _usersDbContext.ReadEmployeeTokens
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (tokenRecord == null)
            {
                return Errors.RefreshTokenByUserIdNotFound();
            }
            return tokenRecord;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
}