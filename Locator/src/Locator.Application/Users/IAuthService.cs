using CSharpFunctionalExtensions;
using Locator.Contracts.Users;
using Shared;

namespace Locator.Application.Users;

public interface IAuthService
{
    Task<Result<(AccessTokenResponse tokenResponse, DateTime createdAt), Error>> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken);
    Task<Result<UserDto, Error>> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken);
    Task<Result<string, Error>> GetValidEmployeeAccessTokenAsync(Guid userId, CancellationToken cancellationToken);
}