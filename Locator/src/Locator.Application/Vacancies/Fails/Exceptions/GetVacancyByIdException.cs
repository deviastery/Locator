using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetVacancyByIdException : FailureException
{
    public GetVacancyByIdException() 
        : base([Errors.General.Failure("Failed to get vacancy by id.")])
    {
    }
}