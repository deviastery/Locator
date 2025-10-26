using Locator.Domain.Thesauruses;

namespace Locator.Contracts.Ratings.Dto;

public record VacancyRatingDto(
    Guid Id,
    double Value,
    long EntityId,
    EntityType EntityType);