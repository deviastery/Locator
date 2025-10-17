using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetVacanciesMatchResumeFailureException : FailureException
{
    public GetVacanciesMatchResumeFailureException() 
        : base([Users.Fails.Errors.General.Failure("Failed to get vacancies that match resume.")])
    {
    }
}