using CSharpFunctionalExtensions;
using HeadHunter.Contracts;
using HeadHunter.Contracts.Dto;
using Shared;
using Shared.Abstractions;
using Users.Application.Fails.Exceptions;
using Users.Contracts.Responses;

namespace Users.Application.GetValidEmployeeTokenByUserIdQuery;

public class GetValidEmployeeTokenByUserId: IQueryHandler<EmployeeTokenResponse, GetValidEmployeeTokenByUserIdQuery>
{
    private readonly IAuthContract _authContract;
    private readonly IQueryHandler<EmployeeTokenResponse, GetEmployeeTokenByUserIdQuery.GetEmployeeTokenByUserIdQuery> 
        _getEmployeeTokenByUserIdQueryHandler;
    private readonly ICommandHandler<Guid, UpdateEmployeeTokenCommand.UpdateEmployeeTokenCommand> 
        _updateEmployeeTokenCommandHandler;


    public GetValidEmployeeTokenByUserId(
        IAuthContract authContract, 
        IQueryHandler<EmployeeTokenResponse, GetEmployeeTokenByUserIdQuery.GetEmployeeTokenByUserIdQuery> getEmployeeTokenByUserIdQueryHandler, 
        ICommandHandler<Guid, UpdateEmployeeTokenCommand.UpdateEmployeeTokenCommand> updateEmployeeTokenCommandHandler)
    {
        _authContract = authContract;
        _getEmployeeTokenByUserIdQueryHandler = getEmployeeTokenByUserIdQueryHandler;
        _updateEmployeeTokenCommandHandler = updateEmployeeTokenCommandHandler;
    }

    public async Task<EmployeeTokenResponse> Handle(GetValidEmployeeTokenByUserIdQuery query, CancellationToken cancellationToken)
    {
        // Get Employee token
        var getEmployeeTokenByUserIdQuery = new GetEmployeeTokenByUserIdQuery.GetEmployeeTokenByUserIdQuery(query.UserId);
        var getEmployeeTokenByUserIdResponse = await _getEmployeeTokenByUserIdQueryHandler.Handle(getEmployeeTokenByUserIdQuery, cancellationToken);

        var tokenDto = getEmployeeTokenByUserIdResponse.EmployeeToken;
        if (tokenDto == null)
        {
            throw new UserUnauthorizedException();
        }

        // Check expired time of Employee access token
        DateTime createdDateTime = tokenDto.CreatedAt;
        DateTime expiredDateTime = createdDateTime.AddSeconds(tokenDto.ExpiresAt);

        // Return Employee access token, if it has not expired
        if (expiredDateTime.ToUniversalTime() > DateTime.UtcNow)
        {
            return new EmployeeTokenResponse(tokenDto);
        }

        // Get a new Employee token, if it has expired
        (_, bool isFailure, EmployeeTokenDto? newEmployeeToken, Error? error) = 
            await _authContract.RefreshTokenAsync(tokenDto, cancellationToken);
        if (isFailure)
        {
            throw new RefreshEmployeeTokenFailureException();
        }

        // Update Employee token
        var updateEmployeeTokenCommand = new UpdateEmployeeTokenCommand.UpdateEmployeeTokenCommand(
            newEmployeeToken);
        var newEmployeeTokenIdResult = await _updateEmployeeTokenCommandHandler.Handle(
            updateEmployeeTokenCommand, 
            cancellationToken);
        if (newEmployeeTokenIdResult.IsFailure)
        {
            throw new SaveEmployeeTokenFailureException();
        }
        
        var dto = new EmployeeTokenDto(
            newEmployeeToken.Id,
            newEmployeeToken.Token,
            newEmployeeToken.RefreshToken,
            newEmployeeToken.CreatedAt,
            newEmployeeToken.ExpiresAt,
            newEmployeeToken.UserId);
        
        return new EmployeeTokenResponse(dto);
    } 
}