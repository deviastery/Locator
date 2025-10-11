using Locator.Application.Abstractions;
using Locator.Application.Users.Fails.Exceptions;
using Locator.Contracts.Users;

namespace Locator.Application.Users.RefreshTokenQuery;

public class RefreshToken: IQueryHandler<RefreshTokenResponse, RefreshTokenQuery>
{
    private readonly IJwtProvider _jwtProvider;
    
    public RefreshToken(IJwtProvider jwtProvider)
    {
        _jwtProvider = jwtProvider;
    }

    public async Task<RefreshTokenResponse?> Handle(RefreshTokenQuery query, CancellationToken cancellationToken)
    {
        var validateRefreshTokenResult = 
            await _jwtProvider.ValidateRefreshTokenAsync(query.RefreshToken, cancellationToken);
        if (validateRefreshTokenResult is null)
        {
            return null;
        }

        string jwtToken = validateRefreshTokenResult.Value.Token;

        return new RefreshTokenResponse(jwtToken);
    }
}