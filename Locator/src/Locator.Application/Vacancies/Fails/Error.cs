using Shared;

namespace Locator.Application.Vacancies.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound("Record not found.", id, "record.not.found");
        public static Error Failure(string message) =>
            Error.Failure($"Something went wrong: {message}", "server.failure");
    }
    public static Error NotReadyForReview() =>
        Error.Validation("Failed to leave a review for the vacancy.", "not.ready.for.review");
    
    public static Error FailGetVacancies() =>
        Error.Failure("Failed to get vacancies.", "fail.get.vacancies");    
    
}