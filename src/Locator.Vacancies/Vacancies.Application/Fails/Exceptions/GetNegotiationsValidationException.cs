using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetNegotiationsValidationException : BadRequestException
{
    public GetNegotiationsValidationException() 
        : base([Errors.General.Validation("Bad request to get negotiations")])
    {
    }
}