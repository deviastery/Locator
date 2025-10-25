using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetNegotiationsValidationException : BadRequestException
{
    public GetNegotiationsValidationException() 
        : base([Errors.General.Validation("Bad request to get negotiations matching the resume")])
    {
    }
}