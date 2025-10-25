using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetVacancyByIdNotFoundException : NotFoundException
{
    public GetVacancyByIdNotFoundException(long id) 
        : base([Errors.General.NotFound(id)])
    {
    }
}