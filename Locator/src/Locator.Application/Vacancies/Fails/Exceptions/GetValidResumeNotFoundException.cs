using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetValidResumeNotFoundException : NotFoundException
{
    public GetValidResumeNotFoundException() 
        : base([Errors.General.Failure("Valid resume not found.")])
    {
    }
}