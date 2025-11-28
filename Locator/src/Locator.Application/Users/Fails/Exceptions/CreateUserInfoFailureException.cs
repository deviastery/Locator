using Locator.Application.Exceptions;

namespace Locator.Application.Users.Fails.Exceptions;

public class CreateUserFailureException : FailureException
{
    public CreateUserFailureException() 
        : base([Errors.General.Failure("Error creating user.")])
    {
    }
}