using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Locator.Application.Users.Fails.Exceptions;
using Locator.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Locator.Application.Users.JwtTokens;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly IUsersRepository _usersRepository;

    public JwtProvider(
        IOptions<JwtOptions> jwtOptions, 
        IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
        _jwtOptions = jwtOptions.Value;
    }

    public (string Token, int ExpiresIn) GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        _ = int.TryParse(_jwtOptions.TokenValidityMins, out int validityMins) ? validityMins : 10;
        var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(validityMins);

        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: tokenExpiryTimeStamp,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(jwt),
            (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds);
    }

    public async Task<string> RefreshAccessTokenAsync(
        string refreshToken, 
        CancellationToken cancellationToken)
    {
        var refreshTokenResult = await _usersRepository.GetRefreshTokenAsync(refreshToken, cancellationToken);
        if (refreshTokenResult.IsFailure || refreshTokenResult.Value is null)
        {
            throw new GetRefreshTokenFailureException();
        }
        var refreshTokenRecord = refreshTokenResult.Value;
        
        if (refreshTokenRecord.ExpiresIn.ToUniversalTime() < DateTime.UtcNow)
        {
            throw new RefreshTokenHasExpiredBadRequestException();
        }
        await _usersRepository.DeleteRefreshTokenAsync(refreshTokenRecord, cancellationToken);
        
        var userResult = await _usersRepository.GetUserAsync(refreshTokenRecord.UserId, cancellationToken);
        if (userResult.IsFailure)
        {
            throw new GetUserFailureException();
        }
        
        var user = userResult.Value;
        if (user is null)
        {
            throw new GetUserNotFoundException();
        }
        
        return GenerateJwtToken(user).Token;
    }

    public async Task<string?> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        _ = int.TryParse(_jwtOptions.RefreshTokenValidityMins, out int validityMins) ? validityMins : 30;
        var refreshToken = new RefreshToken(
            Guid.NewGuid(),
            DateTime.UtcNow.AddMinutes(validityMins),
            userId);
        await _usersRepository.CreateRefreshTokenAsync(refreshToken, cancellationToken);

        return refreshToken.Token.ToString();
    }    
}