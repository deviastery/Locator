using Locator.Application.Abstractions;
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
        var validateRefreshTokenResult = _jwtProvider.ValidateRefreshToken(query.RefreshToken);
        if (validateRefreshTokenResult is null)
        {
            return null;
        }

        var jwtToken = validateRefreshTokenResult.Result?.Item1;

        return new RefreshTokenResponse(jwtToken);
    }
}