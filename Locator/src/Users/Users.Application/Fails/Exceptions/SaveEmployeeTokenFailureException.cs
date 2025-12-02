using Shared.Fails.Exceptions;

namespace Users.Application.Fails.Exceptions;

public class SaveEmployeeTokenFailureException : FailureException
{
    public SaveEmployeeTokenFailureException() 
        : base([Errors.General.Failure("Failed to save employee token.")])
    {
    }
}