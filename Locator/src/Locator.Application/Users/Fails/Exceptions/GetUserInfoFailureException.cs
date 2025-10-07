using Locator.Application.Exceptions;

namespace Locator.Application.Users.Fails.Exceptions;

public class GetUserInfoFailureException : FailureException
{
    public GetUserInfoFailureException() 
        : base([Errors.General.Failure("Error get user info.")])
    {
    }
}