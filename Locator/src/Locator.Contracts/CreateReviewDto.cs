namespace Locator.Contracts;

public record AddReviewDto(double Rating, string Comment, Guid UserId);