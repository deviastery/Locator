using Shared.Fails.Exceptions;

namespace HeadHunter.Fails.Exceptions;

public class UserUnauthorizedException : UnauthorizedException
{
    public UserUnauthorizedException() 
        : base([Errors.General.Unauthorized()])
    {
    }
}