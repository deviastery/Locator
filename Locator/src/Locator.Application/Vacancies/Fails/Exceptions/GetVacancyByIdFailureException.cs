using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetVacancyByIdFailureException : FailureException
{
    public GetVacancyByIdFailureException() 
        : base([Errors.General.Failure("Failed to get vacancy by id.")])
    {
    }
}