using Shared.Fails.Exceptions;

namespace Users.Application.Fails.Exceptions;

public class RefreshEmployeeTokenFailureException : FailureException
{
    public RefreshEmployeeTokenFailureException() 
        : base([Errors.General.Failure("Failed to refresh employee token.")])
    {
    }
}