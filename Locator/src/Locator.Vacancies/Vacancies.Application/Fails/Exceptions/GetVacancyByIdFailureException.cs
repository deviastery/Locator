using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetVacancyByIdFailureException : FailureException
{
    public GetVacancyByIdFailureException() 
        : base([Errors.General.Failure("Failed to get vacancy by id.")])
    {
    }
}