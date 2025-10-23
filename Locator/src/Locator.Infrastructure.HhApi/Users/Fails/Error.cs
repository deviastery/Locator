using Shared;

namespace Locator.Infrastructure.HhApi.Users.Fails;

public partial class Errors
{
    public static class General
    {
        public static Error NotFound<T>(T id) => 
            Error.NotFound($"Record not found.", id, "record.not.found");
        public static Error Failure(string message) =>
            Error.Failure($"Something went wrong: {message}", "server.failure");
    }
    public static Error TokenExchangeFailed() =>
        Error.Failure("Failed to exchange a token.", "token.exchange.fail");
    public static Error InvalidTokenResponse() =>
        Error.Failure("Invalid token response.", "invalid.token.response");        
    public static Error GetUserInfoFailed() =>
        Error.Failure("Failed to get user info.", "user.info.fail");
    public static Error MissingEmail() =>
        Error.Validation("Missing email address.", code: "missing.email");        
    public static Error GetResumesFailed() =>
        Error.Failure("Failed to get resumes.", "resumes.fail");
    public static Error MissingResumes() =>
        Error.Validation("Missing resumes.", "missing.resumes");        
    public static Error GetVacanciesFailed() =>
        Error.Failure("Failed to get vacancies.", "vacancies.fail");
    public static Error MissingVacancies() =>
        Error.Validation("Missing vacancies.", "missing.vacancies");
    public static Error MissingNegotiations() =>
        Error.Validation("Missing negotiations.", "missing.negotiations");        
    public static Error GetNegotiationsFailed() =>
        Error.Failure("Failed to get negotiation.", "negotiation.fail");       
    public static Error EnumQueryValidationFailed() =>
        Error.Validation("Enum query is not valid.", code: "Enum.query.invalid");
}