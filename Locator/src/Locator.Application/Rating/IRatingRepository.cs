using Locator.Domain.Rating;

namespace Locator.Application.Rating;

public interface IRatingRepository
{
    Task<Guid> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken);
}