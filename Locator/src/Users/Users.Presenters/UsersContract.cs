using CSharpFunctionalExtensions;
using Shared;
using Shared.Abstractions;
using Users.Application.Fails;
using Users.Application.GetEmployeeTokenByUserIdQuery;
using Users.Application.GetUserQuery;
using Users.Application.UpdateEmployeeTokenCommand;
using Users.Contracts;
using Users.Contracts.Dto;
using Users.Contracts.Responses;

namespace Users.Presenters;

public class UsersContract : IUsersContract
{
    private readonly IQueryHandler<UserResponse, GetUserQuery> _getUserQueryHandler;
    private readonly IQueryHandler<EmployeeTokenResponse, GetEmployeeTokenByUserIdQuery> _getEmployeeTokenByUserIdHandler;
    private readonly ICommandHandler<Guid, UpdateEmployeeTokenCommand> _updateEmployeeTokenCommandHandler;

    public UsersContract(
        IQueryHandler<UserResponse, GetUserQuery> getUserQueryHandler, 
        ICommandHandler<Guid, UpdateEmployeeTokenCommand> updateEmployeeTokenCommandHandler, 
        IQueryHandler<EmployeeTokenResponse, GetEmployeeTokenByUserIdQuery> getEmployeeTokenByUserIdHandler)
    {
        _getUserQueryHandler = getUserQueryHandler;
        _updateEmployeeTokenCommandHandler = updateEmployeeTokenCommandHandler;
        _getEmployeeTokenByUserIdHandler = getEmployeeTokenByUserIdHandler;
    }
    
    public async Task<UserDto?> GetUserDtoAsync(GetUserDto dto, CancellationToken cancellationToken)
    {
        var getUserQuery = new GetUserQuery(dto);
        var user = await _getUserQueryHandler
            .Handle(getUserQuery, cancellationToken);
        return user.User ?? null;
    }
    
    public async Task<Result<Guid, Error>> UpdateEmployeeTokenAsync(
        EmployeeTokenDto dto, CancellationToken cancellationToken)
    {
        var updateEmployeeTokenCommand = new UpdateEmployeeTokenCommand(dto);
        var newTokenResult = await _updateEmployeeTokenCommandHandler
            .Handle(updateEmployeeTokenCommand, cancellationToken);
        if (newTokenResult.IsFailure)
        {
            return Errors.General.Failure("Update employee token failed");
        }

        var newToken = newTokenResult.Value;
        
        return newToken;
    }
    
    public async Task<EmployeeTokenDto?> GetEmployeeTokenDtoByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        var getEmployeeTokenByUserIdQuery = new GetEmployeeTokenByUserIdQuery(userId);
        var token = await _getEmployeeTokenByUserIdHandler
            .Handle(getEmployeeTokenByUserIdQuery, cancellationToken);
        return token.EmployeeToken ?? null;
    }
}