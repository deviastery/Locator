using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetNegotiationsFailureException : FailureException
{
    public GetNegotiationsFailureException() 
        : base([Errors.General.Failure("Failed to get negotiations.")])
    {
    }
}