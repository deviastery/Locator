using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetVacanciesMatchResumeNotFoundException : NotFoundException
{
    public GetVacanciesMatchResumeNotFoundException(string message) 
        : base([Errors.General.NotFound(message)])
    {
    }
}