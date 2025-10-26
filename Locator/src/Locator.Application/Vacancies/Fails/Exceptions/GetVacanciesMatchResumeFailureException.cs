using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetVacanciesMatchResumeFailureException : FailureException
{
    public GetVacanciesMatchResumeFailureException() 
        : base([Errors.General.Failure("Failed to get vacancies that match resume.")])
    {
    }
}