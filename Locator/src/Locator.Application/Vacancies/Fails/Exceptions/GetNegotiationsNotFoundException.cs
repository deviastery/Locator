using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetNegotiationsNotFoundException : NotFoundException
{
    public GetNegotiationsNotFoundException() 
        : base([Errors.General.NotFound<int?>(null)])
    {
    }
}