using Shared.Fails.Exceptions;

namespace Users.Application.Fails.Exceptions;

public class GetUserInfoFailureException : FailureException
{
    public GetUserInfoFailureException() 
        : base([Errors.General.Failure("Error getting user info.")])
    {
    }
}