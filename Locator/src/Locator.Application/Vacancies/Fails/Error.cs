using Shared;

namespace Locator.Application.Vacancies.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound("record.not.found", "Record not found.", id);
        public static Error Failure(string message) =>
            Error.Failure("server.failure", $"Something went wrong: {message}");
    }
    public static Error NotReadyForReview() =>
        Error.Validation("not.ready.for.review", "Failed to leave a review for the vacancy.");
    
    public static Error FailGetVacancies() =>
        Error.Failure("fail.get.vacancies", "Failed to get vacancies.");    
    
    public static Error FailGetVacancyRatings() =>
        Error.Failure("fail.get.ratings", "Failed to get vacancy ratings.");
}