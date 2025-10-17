using Locator.Infrastructure.HhApi.Exceptions;

namespace Locator.Infrastructure.HhApi.Users.Fails.Exceptions;

public class UserUnauthorizedFailureException : FailureException
{
    public UserUnauthorizedFailureException() 
        : base([Errors.General.Failure("Failed to get refresh token.")])
    {
    }
}