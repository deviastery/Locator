using Shared;

namespace Locator.Application.Vacancies.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound("record.not.found", "Record not found.", id);
    }
    public static Error NotReadyForReview() =>
        Error.Validation("not.ready.for.review", "Failed to leave a review for the vacancy.");
    
    public static Error FailGetVacancy() =>
        Error.Failure("fail.get.vacancy", "Failed to get a vacancy by ID.");
}