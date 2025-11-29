using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Locator.Application.Users.Fails;
using Locator.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.Options;

namespace Locator.Application.Users.JwtTokens;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly IUsersRepository _usersRepository;
    private readonly ITokenCacheService _tokenCache;

    public JwtProvider(
        IOptions<JwtOptions> jwtOptions, 
        IUsersRepository usersRepository,
        ITokenCacheService tokenCache)
    {
        _jwtOptions = jwtOptions.Value;
        _usersRepository = usersRepository;
        _tokenCache = tokenCache;
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
        var refreshToken = await GetRefreshTokenAsync(userId, cancellationToken);
        if (refreshToken is null)
        {
            return Errors.General.Failure("Error getting refresh token.");
        }
        
        // Check expired time of a Refresh token
        DateTime createdDateTime = refreshToken.CreatedAt;
        DateTime expiredDateTime = createdDateTime.AddSeconds(refreshToken.ExpiresAt);
        if (expiredDateTime.ToUniversalTime() > DateTime.UtcNow)
        {
            return Errors.RefreshTokenHasExpired();
        }
        await _usersRepository.DeleteRefreshTokenAsync(refreshToken, cancellationToken);
        
        // Get User
        (_, bool isFailure, User? user) = await _usersRepository.GetUserAsync(refreshToken.UserId, cancellationToken);
        if (isFailure)
        {
            return Errors.General.Failure("Error getting user.");
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
    
    private async Task<RefreshToken?> GetRefreshTokenAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Try to get Refresh token from cache
            var cachedRefreshToken = await _tokenCache.GetRefreshTokenAsync(userId, cancellationToken);
            if (cachedRefreshToken != null)
            {
                return cachedRefreshToken;
            }

            // If Cache miss try to get Refresh token from DB
            (_, bool isFailure, RefreshToken dbRefreshToken) =
                await _usersRepository.GetRefreshTokenByUserIdAsync(userId, cancellationToken);
            if (isFailure)
            {
                return null;
            }

            // Set Refresh token in cache
            if (dbRefreshToken != null)
            {
                await _tokenCache.SetRefreshTokenAsync(dbRefreshToken, cancellationToken);
            }

            return dbRefreshToken;
        }
        catch
        {
            return null;
        }
    }
}