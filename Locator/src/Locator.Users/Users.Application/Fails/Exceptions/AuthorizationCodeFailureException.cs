using Shared.Fails.Exceptions;

namespace Users.Application.Fails.Exceptions;

public class AuthorizationCodeFailureException : FailureException
{
    public AuthorizationCodeFailureException() 
        : base([Errors.General.Failure("Authorization code is not valid.")])
    {
    }
}