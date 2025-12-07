using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetValidResumeNotFoundException : NotFoundException
{
    public GetValidResumeNotFoundException(string message) 
        : base([Errors.General.NotFound(message)])
    {
    }
}