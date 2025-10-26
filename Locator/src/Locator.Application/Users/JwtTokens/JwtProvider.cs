using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Locator.Application.Users.Fails;
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

    public async Task<Result<string, Error>> RefreshAccessTokenAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        // Get Refresh token
        (_, bool isTokenFailure, RefreshToken? refreshTokenRecord, Error? error) =
            await _usersRepository.GetRefreshTokenByUserIdAsync(userId, cancellationToken);
        if (isTokenFailure)
        {
            return error;
        }

        // Check expired time of a Refresh token
        DateTime createdDateTime = refreshTokenRecord.CreatedAt;
        DateTime expiredDateTime = createdDateTime.AddSeconds(refreshTokenRecord.ExpiresAt);
        if (expiredDateTime.ToUniversalTime() > DateTime.UtcNow)
        {
            return Errors.RefreshTokenHasExpired();
        }
        await _usersRepository.DeleteRefreshTokenAsync(refreshTokenRecord, cancellationToken);
        
        // Get User
        (_, bool isFailure, User? user) = await _usersRepository.GetUserAsync(refreshTokenRecord.UserId, cancellationToken);
        if (isFailure)
        {
            return Errors.General.Failure("Error get user.");
        }

        if (user is null)
        {
            return Errors.General.NotFound("User not found");
        }
        
        return GenerateJwtToken(user).Token;
    }

    public async Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        _ = int.TryParse(_jwtOptions.RefreshTokenValidityMins, out int validityMins) ? validityMins : 30;
        var refreshToken = new RefreshToken(
            Guid.NewGuid().ToString(),
            DateTime.UtcNow,
            validityMins,
            userId);
        await _usersRepository.CreateRefreshTokenAsync(refreshToken, cancellationToken);

        return refreshToken.Token;
    }    
}