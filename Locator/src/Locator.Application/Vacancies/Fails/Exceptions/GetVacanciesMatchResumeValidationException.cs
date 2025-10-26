using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetVacanciesMatchResumeValidationException : BadRequestException
{
    public GetVacanciesMatchResumeValidationException(string? message = null) 
        : base([Errors.General.Validation(message ?? "Bad request to get vacancies matching the resume.")])
    {
    }
}