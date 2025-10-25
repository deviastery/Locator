using Shared;

namespace Locator.Infrastructure.HhApi.Users.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error Unauthorized() =>
            Error.Unauthorized("The user is not authorized.");
        public static Error NotFound<T>(T? id) => 
            Error.NotFound("Resource not found", id, "record.not.found");
        public static Error Failure(string message) =>
            Error.Failure($"Something went wrong: {message}", "server.failure");
        public static Error Validation(string message) =>
            Error.Validation(message, code: "value.is.invalid");
    }
    public static Error TokenExchangeFailed() =>
        Error.Failure("Failed to exchange a token.", "token.exchange.fail");
    public static Error InvalidTokenResponse() =>
        Error.Failure("Invalid token response.", "invalid.token.response");   
    public static Error MissingNegotiationByVacancyId(long vacancyId) =>
        Error.Validation($"Missing negotiation by vacancy {vacancyId}.");        
}