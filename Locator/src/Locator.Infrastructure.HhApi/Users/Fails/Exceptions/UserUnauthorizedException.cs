using Locator.Infrastructure.HhApi.Exceptions;

namespace Locator.Infrastructure.HhApi.Users.Fails.Exceptions;

public class UserUnauthorizedException : UnauthorizedException
{
    public UserUnauthorizedException() 
        : base([Errors.General.Unauthorized()])
    {
    }
}