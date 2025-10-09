using Locator.Application.Abstractions;
using Locator.Application.Users.Fails.Exceptions;
using Locator.Contracts.Users;
using Locator.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Locator.Application.Users.AuthQuery;

public class Auth: IQueryHandler<AuthResponse, AuthQuery>
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IUsersRepository _usersRepository;
    private readonly IUsersReadDbContext _usersDbContext;

    public Auth(
        IAuthService authService, 
        ITokenService tokenService, 
        IUsersRepository usersRepository, 
        IUsersReadDbContext usersDbContext)
    {
        _authService = authService;
        _tokenService = tokenService;
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

        // Get info about user
        var userAuthResult = await _authService.GetUserInfoAsync(tokenResult.Value.AccessToken, cancellationToken);
        if (userAuthResult.IsFailure)
        {
            throw new GetUserInfoFailureException();
        }
        var EmployeeUser = userAuthResult.Value;

        // Find user in DB or create new user
        var user = await _usersDbContext.ReadUsers
            .Where(u => u.EmployeeId.ToString() == EmployeeUser.EmployeeId)
            .FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            if (!long.TryParse(EmployeeUser.EmployeeId, out long newEmployeeId))
            {
                throw new EmployeeIdParseFailureException();
            }
            user = new User(
                email: EmployeeUser.Email ?? "",
                name: EmployeeUser.FirstName ?? "",
                employeeId: newEmployeeId
            );
            await _usersRepository.CreateAsync(user, cancellationToken);
        }

        // Generate JWT-token 
        var jwtToken = _tokenService.GenerateToken(user.Id, user.Email);
        return new AuthResponse(jwtToken);
    } 
}