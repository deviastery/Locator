using Shared;

namespace Locator.Application.Users.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error NotFound<T>(T id) =>
            Error.NotFound("record.not.found", "Record not found.", id);
        public static Error Failure(string message) =>
            Error.Failure("server.failure", $"Something went wrong: {message}");
    }
    public static Error TokenExchangeFailed() =>
        Error.Validation("token.exchange.fail", "Failed to exchange a token.");
    public static Error InvalidTokenResponse() =>
        Error.Failure("invalid.token.response", "Invalid token response.");        
    public static Error UserInfoFailed() =>
        Error.Validation("user.info.fail", "Failed to get user info.");
    public static Error MissingEmail() =>
        Error.Failure("missing.email", "Missing email address.");
}