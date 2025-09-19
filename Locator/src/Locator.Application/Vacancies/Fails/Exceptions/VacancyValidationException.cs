using Locator.Application.Exceptions;
using Shared;

namespace Locator.Application.Vacancies.Fails.Exceptions;


public class VacancyValidationException : BadRequestException
{
    public VacancyValidationException(IEnumerable<Error> errors) 
        : base(errors)
    {
    }
}