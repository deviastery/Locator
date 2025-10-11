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

    public async Task<(string Token, int ExpiresIn)?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        var refreshTokenResult = await _usersRepository.GetRefreshTokenAsync(token, cancellationToken);
        if (refreshTokenResult.IsFailure)
        {
            return null;
        }
        
        var refreshToken = refreshTokenResult.Value;
        if (refreshToken is null || refreshToken.ExpiresIn < DateTime.UtcNow)
        {
            return null;
        }
        await _usersRepository.DeleteRefreshTokenAsync(refreshToken, cancellationToken);
        
        var userResult = await _usersRepository.GetUserAsync(refreshToken.UserId, cancellationToken);
        if (userResult.IsFailure)
        {
            throw new GetUserFailureException();
        }
        
        var user = userResult.Value;
        if (user is null)
        {
            throw new GetUserNotFoundException();
        }
        
        return GenerateJwtToken(user);
    }

    public async Task<string?> GenerateRefreshToken(Guid userId, CancellationToken cancellationToken)
    {
        _ = int.TryParse(_jwtOptions.RefreshTokenValidityMins, out int validityMins) ? validityMins : 30;
        var refreshToken = new RefreshToken(
            Guid.NewGuid(),
            DateTime.UtcNow.AddMinutes(validityMins),
            userId
            );
        await _usersRepository.CreateRefreshTokenAsync(refreshToken, cancellationToken);

        return refreshToken.Token.ToString();
    }
}