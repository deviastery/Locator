using Locator.Application.Exceptions;

namespace Locator.Application.Vacancies.Fails.Exceptions;

public class GetValidEmployeeAccessTokenException : FailureException
{
    public GetValidEmployeeAccessTokenException() 
        : base([Users.Fails.Errors.General.Failure("Failed to get valid employee access token.")])
    {
    }
}