using CSharpFunctionalExtensions;
using HeadHunter.Contracts;
using HeadHunter.Contracts.Dto;
using HeadHunter.Contracts.Responses;
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
    private readonly ICommandHandler<Guid, CreateUserCommand.CreateUserCommand> _createUserCommandHandler;
    private readonly IAuthContract _authContract;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUsersRepository _usersRepository;
    private readonly IUsersReadDbContext _usersDbContext;

    public Auth(
        ICommandHandler<Guid, CreateUserCommand.CreateUserCommand> createUserCommandHandler,
        IAuthContract authContract, 
        IJwtProvider jwtProvider, 
        IUsersRepository usersRepository, 
        IUsersReadDbContext usersDbContext)
    {
        _createUserCommandHandler = createUserCommandHandler;
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
        Guid userId;
        if (user == null)
        {
            // Create new user
            if (!long.TryParse(userInfoResult.Value.EmployeeId, out long newEmployeeId))
            {
                throw new EmployeeIdParseFailureException();
            }

            var userDto = new CreateUserDto(
                newEmployeeId,
                userInfoResult.Value.Email ?? string.Empty,
                userInfoResult.Value.FirstName ?? string.Empty);
            var userIdResult = await _createUserCommandHandler.Handle(
                new CreateUserCommand.CreateUserCommand(Guid.NewGuid(), userDto), cancellationToken);
            if (userIdResult.IsFailure)
            {
                throw new CreateUserFailureException();
            }

            userId = userIdResult.Value;
        }
        else
        {
            userId = user.Id;
        }
        
        // Save Employee tokens with User data
        var newEmployeeTokenDto = new EmployeeToken(
            newEmployeeToken.AccessToken, 
            newEmployeeToken.RefreshToken,
            newCreatedAt, 
            newEmployeeToken.ExpiresIn, 
            userId);
        var saveTokensResult = await _usersRepository.CreateEmployeeTokenAsync(newEmployeeTokenDto, cancellationToken);
        if (saveTokensResult.IsFailure)
        {
            throw new SaveEmployeeTokenFailureException();
        }

        // Generate JWT-token 
        (string jwtToken, int tokenExpiry) = _jwtProvider.GenerateJwtToken(userId, user?.Email);

        // Generate Refresh token 
        string refreshToken = await _jwtProvider.GenerateRefreshTokenAsync(userId, cancellationToken);
        
        return new AuthResponse(userId, jwtToken, tokenExpiry, refreshToken);
    } 
}