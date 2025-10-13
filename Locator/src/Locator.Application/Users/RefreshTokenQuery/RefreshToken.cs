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

    public async Task<RefreshTokenResponse> Handle(RefreshTokenQuery query, CancellationToken cancellationToken)
    {
        var jwtToken = 
            await _jwtProvider.RefreshAccessTokenAsync(query.RefreshToken, cancellationToken);

        return new RefreshTokenResponse(jwtToken);
    }
}