using Shared.Fails.Exceptions;

namespace Users.Application.Fails.Exceptions;

public class CreateUserFailureException : FailureException
{
    public CreateUserFailureException() 
        : base([Errors.General.Failure("Error creating user.")])
    {
    }
}