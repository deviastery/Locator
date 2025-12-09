using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetNegotiationsNotFoundException : NotFoundException
{
    public GetNegotiationsNotFoundException(string message) 
        : base([Errors.General.NotFound(message)])
    {
    }
}