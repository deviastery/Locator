using CSharpFunctionalExtensions;
using Locator.Application.Abstractions;
using Locator.Application.Users.Fails.Exceptions;
using Locator.Contracts.Users.Dtos;
using Locator.Contracts.Users.Responses;
using Locator.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Locator.Application.Users.AuthQuery;

public class Auth: IQueryHandler<AuthResponse, AuthQuery>
{
    private readonly IAuthService _authService;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUsersRepository _usersRepository;
    private readonly IUsersReadDbContext _usersDbContext;

    public Auth(
        IAuthService authService, 
        IJwtProvider jwtProvider, 
        IUsersRepository usersRepository, 
        IUsersReadDbContext usersDbContext)
    {
        _authService = authService;
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

        // Get Token from another API
        var tokenResult = await _authService.ExchangeCodeForTokenAsync(query.Dto.Code, cancellationToken);
        if (tokenResult.IsFailure)
        {
            throw new ExchangeCodeForTokenFailureException();
        }
        (AccessTokenResponse newEmployeeToken, DateTime newCreatedAt) = tokenResult.Value;

        // Get info about user
        (_, bool isFailure, UserDto? employeeUser) = await _authService.GetUserInfoAsync(newEmployeeToken.AccessToken, cancellationToken);
        if (isFailure)
        {
            throw new GetUserInfoFailureException();
        }

        // Find user in DB or create new user
        var user = await _usersDbContext.ReadUsers
            .Where(u => u.EmployeeId.ToString() == employeeUser.EmployeeId)
            .FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            if (!long.TryParse(employeeUser.EmployeeId, out long newEmployeeId))
            {
                throw new EmployeeIdParseFailureException();
            }
            user = new User(
                email: employeeUser.Email ?? string.Empty,
                name: employeeUser.FirstName ?? string.Empty,
                employeeId: newEmployeeId);
            await _usersRepository.CreateUserAsync(user, cancellationToken);
        }
        
        // Save employee tokens with user data
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

        // Generate JWT-token 
        string refreshToken = await _jwtProvider.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        
        return new AuthResponse(user.Name, jwtToken, tokenExpiry, refreshToken);
    } 
}