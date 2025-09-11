namespace Locator.Contracts.Vacancies;

public record CreateReviewDto(double Mark, string? Comment, Guid UserId);