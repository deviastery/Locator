using Locator.Application.Exceptions;

namespace Locator.Application.Users.Fails.Exceptions;

public class RefreshTokenHasExpiredBadRequestException : BadRequestException
{
    public RefreshTokenHasExpiredBadRequestException() 
        : base([Errors.General.Failure("Refresh token has expired.")])
    {
    }
}