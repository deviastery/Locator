using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Locator.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;

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

    public (string, int) GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        _ = int.TryParse(_jwtOptions.TokenValidityMins, out var validityMins) ? validityMins : 10;
        var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(validityMins);

        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: tokenExpiryTimeStamp,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(jwt),
            (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds);
    }

    public async Task<(string, int)?> ValidateRefreshToken(string token)
    {
        var refreshToken = await _usersRepository.GetRefreshTokenAsync(token);
        if (refreshToken is null || refreshToken.ExpiresIn < DateTime.UtcNow)
        {
            return null;
        }
        await _usersRepository.DeleteRefreshTokenAsync(refreshToken);
        
        var user = _usersRepository.GetUserAsync(refreshToken.UserId);
        if (user is null)
        {
            return null;
        }
        
        return GenerateJwtToken(user);
    }

    public async Task<string?> GenerateRefreshToken(Guid userId, CancellationToken cancellationToken)
    {
        _ = int.TryParse(_jwtOptions.RefreshTokenValidityMins, out var validityMins) ? validityMins : 30;
        var refreshToken = new RefreshToken(
            Guid.NewGuid().ToString(),
            DateTime.UtcNow.AddMinutes(validityMins),
            userId
            );
        await _usersRepository.CreateRefreshTokenAsync(refreshToken, cancellationToken);

        return refreshToken.Token;
    }
}