using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetVacanciesMatchResumeFailureException : FailureException
{
    public GetVacanciesMatchResumeFailureException() 
        : base([Errors.General.Failure("Failed to get vacancies that match resume.")])
    {
    }
}