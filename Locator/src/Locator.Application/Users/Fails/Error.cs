using Shared;

namespace Locator.Application.Users.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error Validation() =>
            Error.Validation("Validation error.");
        public static Error Unauthorized() =>
            Error.Unauthorized("The user is not authorized.");
        public static Error NotFound<T>(T id) =>
            Error.NotFound("Record not found.", id, "record.not.found");
        public static Error Failure(string message) =>
            Error.Failure($"Something went wrong: {message}", "server.failure");
    }
    public static Error RefreshTokenHasExpired() =>
        Error.Validation("Refresh token has expired.", "token.has.for.expired");
}