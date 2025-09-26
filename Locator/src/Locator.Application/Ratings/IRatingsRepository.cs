using CSharpFunctionalExtensions;
using Locator.Domain.Ratings;
using Shared;

namespace Locator.Application.Ratings;

public interface IRatingsRepository
{
    Task<Guid> UpdateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken);
    Task<Result<VacancyRating?, Error>> GetVacancyRatingByIdAsync(Guid ratingId, CancellationToken cancellationToken);
    Task<Result<Dictionary<Guid, double?>, Error>> GetVacancyRatingsByIdsAsync(IEnumerable<Guid> ratingIds, CancellationToken cancellationToken);
}