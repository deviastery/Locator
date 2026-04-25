using Shared;

namespace Users.Application.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error Validation() =>
            Error.Validation("Validation error.");
        public static Error Unauthorized() =>
            Error.Unauthorized("The user is not authorized.");
        public static Error NotFound(string? message = null) =>
            Error.NotFound(message ?? "Record not found.", "record.not.found");
        public static Error Failure(string message) =>
            Error.Failure($"Something went wrong: {message}", "server.failure");
    }
    public static Error RefreshTokenHasExpired() =>
        Error.Validation("Refresh token has expired.", "token.has.for.expired");
    public static Error RefreshTokenByUserIdNotFound() =>
        Error.NotFound("Refresh token by user id not found.", "refresh.token.not.found"); 
    public static Error CreateUserFailure() =>
        Error.Failure("Error creating user.");  
}