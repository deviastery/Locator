using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetRatingsFailureException : FailureException
{
    public GetRatingsFailureException() 
        : base([Errors.General.Failure("Failed to get vacancy ratings.")])
    {
    }
}