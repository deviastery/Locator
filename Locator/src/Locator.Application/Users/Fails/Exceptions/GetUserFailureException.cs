using Locator.Application.Exceptions;

namespace Locator.Application.Users.Fails.Exceptions;

public class GetUserFailureException : FailureException
{
    public GetUserFailureException() 
        : base([Errors.General.Failure("Error get user.")])
    {
    }
}