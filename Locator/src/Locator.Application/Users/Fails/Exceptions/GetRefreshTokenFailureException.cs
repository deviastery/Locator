using Locator.Application.Exceptions;

namespace Locator.Application.Users.Fails.Exceptions;

public class GetRefreshTokenFailureException : FailureException
{
    public GetRefreshTokenFailureException() 
        : base([Errors.General.Failure("Failed to get refresh token.")])
    {
    }
}