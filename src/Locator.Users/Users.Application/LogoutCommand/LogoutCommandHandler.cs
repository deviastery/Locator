using CSharpFunctionalExtensions;
using Redis.Contracts;
using Shared;
using Shared.Abstractions;
using Users.Application.JwtTokens;

namespace Users.Application.LogoutCommand;

public class LogoutCommandHandler: ICommandHandler<LogoutCommand>
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IUsersRepository _usersRepository;
    private readonly ITokenCacheContract _tokenCache;
    
    public LogoutCommandHandler(
        IJwtProvider jwtProvider, 
        IUsersRepository usersRepository, 
        ITokenCacheContract tokenCache)
    {
        _jwtProvider = jwtProvider;
        _usersRepository = usersRepository;
        _tokenCache = tokenCache;
    }

    public async Task<UnitResult<Failure>> Handle(
        LogoutCommand command, 
        CancellationToken cancellationToken)
    {
        // Delete refresh and employee tokens from Cache
        await _tokenCache.InvalidateTokensAsync(command.UserId, cancellationToken);

        // Delete all refresh tokens by user ID from DB
        var refreshTokensResult = await _jwtProvider.DeleteRefreshTokensByUserId(command.UserId, cancellationToken);
        if (refreshTokensResult.IsFailure)
            return refreshTokensResult.Error.ToFailure();
        
        // Delete all employee tokens by user ID from DB
        var employeeTokensResult = await _usersRepository.DeleteEmployeeTokensByUserIdAsync(command.UserId, cancellationToken);
        if (employeeTokensResult.IsFailure)
            return employeeTokensResult.Error.ToFailure();
        
        return UnitResult.Success<Failure>();
    }
}