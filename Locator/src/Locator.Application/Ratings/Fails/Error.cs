using Shared;

namespace Locator.Application.Ratings.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error NotFound<T>(T id) => 
            Error.NotFound($"Record not found.", id, "record.not.found");
        public static Error Failure(string message) =>
            Error.Failure($"Something went wrong: {message}", "server.failure");
    }
    
    public static Error FailGetVacancyRatings() =>
        Error.Failure("Failed to get vacancy ratings.", "fail.get.ratings");
}