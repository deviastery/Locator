using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetVacancyByIdNotFoundException : NotFoundException
{
    public GetVacancyByIdNotFoundException(string message) 
        : base([Errors.General.NotFound(message)])
    {
    }
}