using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetVacancyByIdNotFoundException : NotFoundException
{
    public GetVacancyByIdNotFoundException(string message) 
        : base([Errors.General.NotFound(message)])
    {
    }
}