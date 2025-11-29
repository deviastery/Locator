using CSharpFunctionalExtensions;
using Locator.Application.Abstractions;
using Locator.Application.Users.JwtTokens;
using Shared;

namespace Locator.Application.Users.RefreshTokenCommand;

public class RefreshTokenCommandHandler: ICommandHandler<string, RefreshTokenCommand>
{
    private readonly IJwtProvider _jwtProvider;
    
    public RefreshTokenCommandHandler(IJwtProvider jwtProvider)
    {
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<string, Failure>> Handle(
        RefreshTokenCommand command, 
        CancellationToken cancellationToken)
    {
        var jwtTokenResult = 
            await _jwtProvider.RefreshAccessTokenAsync(command.UserId, cancellationToken);
        if (jwtTokenResult.IsFailure)
        {
            jwtTokenResult.Error.ToFailure();
        }

        return jwtTokenResult.Value;
    }
}