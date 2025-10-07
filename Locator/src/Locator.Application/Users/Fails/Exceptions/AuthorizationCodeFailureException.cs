using Locator.Application.Exceptions;

namespace Locator.Application.Users.Fails.Exceptions;

public class AuthorizationCodeFailureException : FailureException
{
    public AuthorizationCodeFailureException() 
        : base([Errors.General.Failure("Authorization code is not valid.")])
    {
    }
}