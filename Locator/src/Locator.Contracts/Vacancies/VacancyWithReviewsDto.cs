using Locator.Domain.Ratings;

namespace Locator.Contracts.Vacancies;

public record VacancyWithReviewsDto(
    Guid Id,
    string Name,
    string Description,
    int? Salary,
    int? Experience,
    VacancyRating? Rating,
    IEnumerable<ReviewDto>? Reviews);