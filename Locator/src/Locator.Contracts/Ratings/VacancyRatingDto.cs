using Locator.Domain.Thesauruses;

namespace Locator.Contracts.Ratings;

public record VacancyRatingDto(
    Guid Id,
    double Value,
    long EntityId,
    EntityType EntityType);