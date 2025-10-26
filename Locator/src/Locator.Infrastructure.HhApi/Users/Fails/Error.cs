using Shared;

namespace Locator.Infrastructure.HhApi.Users.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error Unauthorized() =>
            Error.Unauthorized("The user is not authorized.");
        public static Error NotFound(string? message = null) => 
            Error.NotFound(message ?? "Resource not found", "record.not.found");
        public static Error Failure(string message) =>
            Error.Failure($"Something went wrong: {message}", "server.failure");
        public static Error Validation(string message) =>
            Error.Validation(message, "value.is.invalid");
    }
    public static Error TokenExchangeFailed() =>
        Error.Failure("Failed to exchange a token.", "token.exchange.fail");
    public static Error InvalidTokenResponse() =>
        Error.Failure("Invalid token response.", "invalid.token.response");   
}