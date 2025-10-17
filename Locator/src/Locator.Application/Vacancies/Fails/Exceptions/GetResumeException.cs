using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetResumeException : FailureException
{
    public GetResumeException() 
        : base([Users.Fails.Errors.General.Failure("Failed to get resume.")])
    {
    }
}