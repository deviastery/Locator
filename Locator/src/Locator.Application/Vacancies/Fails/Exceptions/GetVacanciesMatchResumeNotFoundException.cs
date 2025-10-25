using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetVacanciesMatchResumeNotFoundException : NotFoundException
{
    public GetVacanciesMatchResumeNotFoundException() 
        : base([Errors.General.NotFound<int?>(null)])
    {
    }
}