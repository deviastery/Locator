using CSharpFunctionalExtensions;
using Ratings.Domain;
using Shared;

namespace Ratings.Application;

public interface IRatingsRepository
{
    /// <summary>
    /// Updates vacancy rating
    /// </summary>
    /// <param name="ratingDto">Vacancy rating</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of updated vacancy rating</returns>
    Task<Result<Guid, Error>> UpdateVacancyRatingAsync(VacancyRating ratingDto, CancellationToken cancellationToken);
    
    /// <summary>
    /// Creates a new vacancy rating
    /// </summary>
    /// <param name="rating">Vacancy rating</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of created vacancy rating</returns>
    Task<Result<Guid, Error>> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets a Vacancy Rating
    /// </summary>
    /// <param name="vacancyId">ID of Vacancy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Vacancy Rating</returns>
    Task<Result<VacancyRating, Error>> GetRatingByVacancyIdAsync(long vacancyId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all Vacancy Ratings
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of Vacancy Ratings</returns>
    Task<Result<VacancyRating[], Error>> GetRatingsAsync(CancellationToken cancellationToken);
}