using Locator.Domain.Rating;

namespace Locator.Application.Rating;

public interface IRatingsRepository
{
    Task<Guid> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken);
}