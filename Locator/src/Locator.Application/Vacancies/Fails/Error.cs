using Shared;

namespace Locator.Application.Vacancies.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error Validation(string message) =>
            Error.Validation(message, "value.is.invalid");
        public static Error NotFound(string? message = null) =>
            Error.NotFound(message ?? "Record not found.", "record.not.found");
        public static Error Failure(string message) =>
            Error.Failure($"Something went wrong: {message}", "server.failure");
    }
    public static Error NotReadyForReview() =>
        Error.Validation("Failed to leave a review for the vacancy", "not.ready.for.review");
    public static Error UserAlreadyReviewedVacancy() =>
        Error.Validation(
            "The user has already left a review for this vacancy", 
            "review.already.left");
    public static Error TryParseNegotiationIdFail() =>
        Error.Failure("Failed to parse negotiation ID");
}