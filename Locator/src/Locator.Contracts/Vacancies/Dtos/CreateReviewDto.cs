namespace Locator.Contracts.Vacancies.Dtos;

public record CreateReviewDto(long NegotiationId, double Mark, string? Comment, string UserName);
