using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetRatingByVacancyIdNotFoundException : NotFoundException
{
    public GetRatingByVacancyIdNotFoundException(string message) 
        : base([Errors.General.NotFound(message)])
    {
    }
}