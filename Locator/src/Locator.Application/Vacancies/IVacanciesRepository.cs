using Locator.Domain.Vacancies;

namespace Locator.Application.Vacancies;

public interface IVacanciesRepository
{
    /// <summary>
    /// Gets information about whether the user has left a review for a vacancy or not
    /// </summary>
    /// <param name="userId">ID of User</param>
    /// <param name="vacancyId">ID of vacancy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Did the user leave a review for the vacancy or not in Bool</returns>
    Task<bool> HasUserReviewedVacancyAsync(
        Guid userId, 
        long vacancyId, 
        CancellationToken cancellationToken);    

    /// <summary>
    /// Creates review of vacancy
    /// </summary>
    /// <param name="review">Review of vacancy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of a new review</returns>
    Task<Guid> CreateReviewAsync(Review review, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets all reviews of a vacancy
    /// </summary>
    /// <param name="vacancyId">ID of vacancy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of reviews</returns>
    Task<List<Review>> GetReviewsByVacancyIdAsync(long vacancyId, CancellationToken cancellationToken);
}