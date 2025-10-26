using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetValidResumeNotFoundException : NotFoundException
{
    public GetValidResumeNotFoundException(string message) 
        : base([Errors.General.NotFound(message)])
    {
    }
}