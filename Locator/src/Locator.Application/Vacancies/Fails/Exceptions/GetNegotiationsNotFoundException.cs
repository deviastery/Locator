using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetNegotiationsNotFoundException : NotFoundException
{
    public GetNegotiationsNotFoundException(string message) 
        : base([Errors.General.NotFound(message)])
    {
    }
}