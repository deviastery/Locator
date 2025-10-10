using Shared;

namespace Locator.Application.Users.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error NotFound<T>(T id) =>
            Error.NotFound("Record not found.", id, "record.not.found");
        public static Error Failure(string message) =>
            Error.Failure($"Something went wrong: {message}", "server.failure");
    }
    public static Error TokenExchangeFailed() =>
        Error.Validation("Failed to exchange a token.", "token.exchange.fail");
    public static Error InvalidTokenResponse() =>
        Error.Failure("Invalid token response.", "invalid.token.response");        
    public static Error UserInfoFailed() =>
        Error.Validation("Failed to get user info.", "user.info.fail");
    public static Error MissingEmail() =>
        Error.Failure("Missing email address.", "missing.email");
}