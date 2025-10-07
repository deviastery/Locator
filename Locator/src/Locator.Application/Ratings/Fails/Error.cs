using Shared;

namespace Locator.Application.Ratings.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error NotFound<T>(T id) => 
            Error.NotFound("record.not.found", $"Record not found.", id);
        public static Error Failure(string message) =>
            Error.Failure("server.failure", $"Something went wrong: {message}");
    }
    
    public static Error FailGetVacancyRatings() =>
        Error.Failure("fail.get.ratings", "Failed to get vacancy ratings.");
}