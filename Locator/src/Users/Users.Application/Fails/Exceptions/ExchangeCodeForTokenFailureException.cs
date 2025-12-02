using Shared.Fails.Exceptions;

namespace Users.Application.Fails.Exceptions;

public class ExchangeCodeForTokenFailureException : FailureException
{
    public ExchangeCodeForTokenFailureException() 
        : base([Errors.General.Failure("Error exchanging code for token.")])
    {
    }
}