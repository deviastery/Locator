using Locator.Contracts.Ratings;

namespace Locator.Application.Ratings;

public interface IRatingsService
{
    Task<Guid> CreateVacancyRating(
        CreateVacancyRatingDto vacancyRatingDto,
        CancellationToken cancellationToken);
}