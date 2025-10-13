using Locator.Application.Exceptions;

namespace Locator.Application.Users.Fails.Exceptions;

public class SaveEmployeeTokenFailureException : FailureException
{
    public SaveEmployeeTokenFailureException() 
        : base([Errors.General.Failure("Failed to save employee token.")])
    {
    }
}