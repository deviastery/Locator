using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetVacanciesMatchResumeNotFoundException : NotFoundException
{
    public GetVacanciesMatchResumeNotFoundException(string message) 
        : base([Errors.General.NotFound(message)])
    {
    }
}