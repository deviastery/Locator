using Locator.Application.Exceptions;

namespace Locator.Application.Users.Fails.Exceptions;

public class ExchangeCodeForTokenFailureException : FailureException
{
    public ExchangeCodeForTokenFailureException() 
        : base([Errors.General.Failure("Error exchange code for token.")])
    {
    }
}