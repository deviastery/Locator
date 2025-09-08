using Locator.Domain.Vacancies;

namespace Locator.Contracts.Rating;

public record CreateVacancyRatingDto(List<Review> Reviews);