using CSharpFunctionalExtensions;
using Locator.Domain.Ratings;
using Shared;

namespace Locator.Application.Ratings;

public interface IRatingsRepository
{
    Task<Result<Guid, Error>> UpdateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken);
    Task<Result<Guid, Error>> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken);
    Task<Result<VacancyRating, Error>> GetVacancyRatingByIdAsync(Guid ratingId, CancellationToken cancellationToken);
}