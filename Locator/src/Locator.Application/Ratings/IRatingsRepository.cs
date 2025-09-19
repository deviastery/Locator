using Locator.Domain.Ratings;

namespace Locator.Application.Ratings;

public interface IRatingsRepository
{
    Task<Guid> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken);
}