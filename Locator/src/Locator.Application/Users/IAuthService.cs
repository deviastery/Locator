using CSharpFunctionalExtensions;
using Locator.Contracts.Users;
using Shared;

namespace Locator.Application.Users;

public interface IAuthService
{
    Task<Result<AccessTokenResponse, Error>> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken);
    Task<Result<UserDto, Error>> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken);
}