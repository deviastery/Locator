using Locator.Contracts.Rating;

namespace Locator.Application.Rating;

public interface IRatingService
{
    Task<Guid> CreateVacancyRating(
        Guid vacancyId,
        CreateVacancyRatingDto reviewsVacancyId,
        CancellationToken cancellationToken);
}