namespace Locator.Contracts.Vacancies.Dto;

public record CreateReviewDto(long NegotiationId, double Mark, string? Comment, string UserName);
