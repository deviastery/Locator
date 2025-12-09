using Shared.Fails.Exceptions;

namespace Vacancies.Application.Fails.Exceptions;

public class GetValidEmployeeAccessTokenException : FailureException
{
    public GetValidEmployeeAccessTokenException() 
        : base([Errors.General.Failure("Failed to get valid employee access token.")])
    {
    }
}