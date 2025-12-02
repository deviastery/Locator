using CSharpFunctionalExtensions;
using Shared;
using Shared.Abstractions;
using Users.Application.JwtTokens;

namespace Users.Application.RefreshTokenCommand;

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