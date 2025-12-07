using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetVacanciesMatchResumeValidationException : BadRequestException
{
    public GetVacanciesMatchResumeValidationException(string? message = null) 
        : base([Errors.General.Validation(message ?? "Bad request to get vacancies matching the resume.")])
    {
    }
}