using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetResumeFailureException : FailureException
{
    public GetResumeFailureException() 
        : base([Errors.General.Failure("Failed to get resume.")])
    {
    }
}