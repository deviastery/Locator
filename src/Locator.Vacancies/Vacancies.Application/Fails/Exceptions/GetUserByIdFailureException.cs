using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetUserByIdFailureException : FailureException
{
    public GetUserByIdFailureException() 
        : base([Errors.General.Failure("Failed to get user by ID")])
    {
    }
}