using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetNegotiationsFailureException : FailureException
{
    public GetNegotiationsFailureException() 
        : base([Errors.General.Failure("Failed to get negotiations by user Id.")])
    {
    }
}