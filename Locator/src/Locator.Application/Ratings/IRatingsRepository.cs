using CSharpFunctionalExtensions;
using Locator.Domain.Ratings;
using Shared;

namespace Locator.Application.Ratings;

public interface IRatingsRepository
{
    /// <summary>
    /// Method for update vacancy rating
    /// </summary>
    /// <param name="rating">Vacancy rating</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of updated vacancy rating</returns>
    Task<Result<Guid, Error>> UpdateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method for create a new vacancy rating
    /// </summary>
    /// <param name="rating">Vacancy rating</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of created vacancy rating</returns>
    Task<Result<Guid, Error>> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken);
}