using Locator.Domain.Vacancies;

namespace Locator.Application.Vacancies;

public interface IVacanciesRepository
{
    /// <summary>
    /// Method for creating review of vacancy
    /// </summary>
    /// <param name="review">Review of vacancy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of a new review</returns>
    Task<Guid> CreateReviewAsync(Review review, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method for getting all reviews of the vacancy
    /// </summary>
    /// <param name="vacancyId">ID of vacancy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of reviews</returns>
    Task<List<Review>> GetReviewsByVacancyIdAsync(long vacancyId, CancellationToken cancellationToken);
}