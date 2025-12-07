using Shared.Fails.Exceptions;

namespace Users.Application.Fails.Exceptions;

public class UserUnauthorizedException : UnauthorizedException
{
    public UserUnauthorizedException() 
        : base([Errors.General.Unauthorized()])
    {
    }
}