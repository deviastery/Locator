using Locator.Domain.Thesauruses;

namespace Locator.Contracts.Ratings;

public record VacancyRatingDto(
    Guid Id,
    double Value,
    Guid EntityId,
    EntityType EntityType);