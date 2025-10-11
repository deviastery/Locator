using Locator.Application.Exceptions;

namespace Locator.Application.Users.Fails.Exceptions;

public class GetUserNotFoundException : FailureException
{
    public GetUserNotFoundException() 
        : base([Errors.General.Failure("User not found.")])
    {
    }
}