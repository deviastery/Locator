using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetResumeFailureException : FailureException
{
    public GetResumeFailureException() 
        : base([Errors.General.Failure("Failed to get resume.")])
    {
    }
}