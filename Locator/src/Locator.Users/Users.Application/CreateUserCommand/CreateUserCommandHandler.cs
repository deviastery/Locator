using CSharpFunctionalExtensions;
using Shared;
using Shared.Abstractions;
using Users.Application.Fails;
using Users.Application.JwtTokens;
using Users.Domain;

namespace Users.Application.CreateUserCommand;

public class CreateUserCommandHandler: ICommandHandler<Guid, CreateUserCommand>
{
    private readonly IUsersRepository _usersRepository;
    
    public CreateUserCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Result<Guid, Failure>> Handle(
        CreateUserCommand command, 
        CancellationToken cancellationToken)
    {
        var user = new User(
            email: command.Dto.Email ?? string.Empty,
            name: command.Dto.FirstName ?? string.Empty,
            employeeId: command.Dto.EmployeeId);
        var userIdResult = await _usersRepository.CreateUserAsync(user, cancellationToken);
        if (userIdResult.IsFailure)
        {
            return Errors.CreateUserFailure().ToFailure();
        }

        return userIdResult.Value;
    }
}