using CSharpFunctionalExtensions;
using HeadHunter.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Abstractions;
using Users.Application.Fails.Exceptions;
using Users.Application.JwtTokens;
using Users.Contracts.Dto;
using Users.Contracts.Responses;
using Users.Domain;

namespace Users.Application.AuthQuery;

public class Auth: IQueryHandler<AuthResponse, AuthQuery>
{
    private readonly IAuthContract _authContract;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUsersRepository _usersRepository;
    private readonly IUsersReadDbContext _usersDbContext;

    public Auth(
        IAuthContract authContract, 
        IJwtProvider jwtProvider, 
        IUsersRepository usersRepository, 
        IUsersReadDbContext usersDbContext)
    {
        _authContract = authContract;
        _jwtProvider = jwtProvider;
        _usersRepository = usersRepository;
        _usersDbContext = usersDbContext;
    }

    public async Task<AuthResponse> Handle(AuthQuery query, CancellationToken cancellationToken)
    {   
        // Validation of input data
        if (string.IsNullOrEmpty(query.Dto.Code))
        {
            throw new AuthorizationCodeFailureException();
        }

        // Get Employee token from another API
        var tokenResult = await _authContract.ExchangeCodeForTokenAsync(query.Dto.Code, cancellationToken);
        if (tokenResult.IsFailure)
        {
            throw new ExchangeCodeForTokenFailureException();
        }
        (AccessTokenResponse newEmployeeToken, DateTime newCreatedAt) = tokenResult.Value;

        // Get info about user
        Result<UserDto, Error> userInfoResult = 
            await _authContract.GetUserInfoAsync(newEmployeeToken.AccessToken, cancellationToken);
        if (userInfoResult.IsFailure)
        {
            throw new GetUserInfoFailureException();
        }

        // Find user in DB
        var user = await _usersDbContext.ReadUsers
            .Where(u => u.EmployeeId.ToString() == userInfoResult.Value.EmployeeId)
            .FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            // Create new user
            if (!long.TryParse(userInfoResult.Value.EmployeeId, out long newEmployeeId))
            {
                throw new EmployeeIdParseFailureException();
            }
            user = new User(
                email: userInfoResult.Value.Email ?? string.Empty,
                name: userInfoResult.Value.FirstName ?? string.Empty,
                employeeId: newEmployeeId);
            var userIdResult = await _usersRepository.CreateUserAsync(user, cancellationToken);
            if (userIdResult.IsFailure)
            {
                throw new CreateUserFailureException();
            }
        }
        
        // Save Employee tokens with User data
        var newEmployeeTokenDto = new EmployeeToken(
            newEmployeeToken.AccessToken, 
            newEmployeeToken.RefreshToken,
            newCreatedAt, 
            newEmployeeToken.ExpiresIn, 
            user.Id);
        var saveTokensResult = await _usersRepository.CreateEmployeeTokenAsync(newEmployeeTokenDto, cancellationToken);
        if (saveTokensResult.IsFailure)
        {
            throw new SaveEmployeeTokenFailureException();
        }

        // Generate JWT-token 
        (string jwtToken, int tokenExpiry) = _jwtProvider.GenerateJwtToken(user);

        // Generate Refresh token 
        string refreshToken = await _jwtProvider.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        
        return new AuthResponse(user.Id, jwtToken, tokenExpiry, refreshToken);
    } 
}