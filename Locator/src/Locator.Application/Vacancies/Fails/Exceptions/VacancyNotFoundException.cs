using Locator.Application.Exceptions;
using Shared;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class VacancyNotFoundException : NotFoundException
{
    public VacancyNotFoundException(IEnumerable<Error> errors) 
        : base(errors)
    {
    }
}