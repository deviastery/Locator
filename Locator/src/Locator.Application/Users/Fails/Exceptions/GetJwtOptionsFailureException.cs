using Locator.Application.Exceptions;

namespace Locator.Application.Users.Fails.Exceptions;

public class GetJwtOptionsFailureException : FailureException
{
    public GetJwtOptionsFailureException() 
        : base([Errors.General.Failure("Failed to get jwt options.")])
    {
    }
}