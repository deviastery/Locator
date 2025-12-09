using CSharpFunctionalExtensions;
using Shared;
using Shared.Abstractions;
using Users.Domain;

namespace Users.Application.UpdateEmployeeTokenCommand;

public class UpdateEmployeeTokenCommandHandler: ICommandHandler<Guid, UpdateEmployeeTokenCommand>
{
    private readonly IUsersRepository _usersRepository;
    
    public UpdateEmployeeTokenCommandHandler(
        IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Result<Guid, Failure>> Handle(
        UpdateEmployeeTokenCommand command, 
        CancellationToken cancellationToken)
    {
        var token = new EmployeeToken(
            command.Token.Token,
            command.Token.RefreshToken,
            command.Token.CreatedAt,
            command.Token.ExpiresAt,
            command.Token.UserId);
        
        var updateTokenResult = await _usersRepository.UpdateEmployeeTokenAsync(token, cancellationToken);
        if (updateTokenResult.IsFailure)
        {
            updateTokenResult.Error.ToFailure();
        }

        return token.Id;
    }
}