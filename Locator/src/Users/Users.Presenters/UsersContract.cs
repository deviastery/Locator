using CSharpFunctionalExtensions;
using Shared;
using Users.Application;
using Users.Contracts;
using Users.Contracts.Dto;
using Users.Domain;

namespace Users.Presenters;

public class UsersContract : IUsersContract
{
    private readonly IUsersRepository _userRepository;

    public UsersContract(
        IUsersRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<UserDto, Error>> GetUserDtoAsync(Guid userId, CancellationToken cancellationToken)
    {
        (_, bool isFailure, User? user, Error? error) = await _userRepository.GetUserAsync(userId, cancellationToken);
        if (isFailure)
        {
            return error;
        }

        var dto = new UserDto(user.EmployeeId.ToString(), user.Name, user.Email);
        return dto;
    }
    
    public async Task<Result<Guid, Error>> UpdateEmployeeTokenAsync(
        EmployeeTokenDto token, CancellationToken cancellationToken)
    {
        var dto = new EmployeeToken(
            token.Token,
            token.RefreshToken,
            token.CreatedAt,
            token.ExpiresAt,
            token.UserId);
        
        return await _userRepository.UpdateEmployeeTokenAsync(dto, cancellationToken);
    }
    
    public async Task<Result<EmployeeTokenDto, Error>> GetEmployeeTokenDtoByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        (_, bool isFailure, EmployeeToken? token, Error? error) = await _userRepository.GetEmployeeTokenByUserIdAsync(userId, cancellationToken);
        if (isFailure)
        {
            return error;
        }

        var dto = new EmployeeTokenDto(
            token.Id,
            token.Token,
            token.RefreshToken,
            token.CreatedAt,
            token.ExpiresAt,
            token.UserId);
        return dto;
    }
}