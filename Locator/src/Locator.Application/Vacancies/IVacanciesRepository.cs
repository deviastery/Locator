using Locator.Contracts.Vacancies;
using Locator.Domain.Rating;
using Locator.Domain.Vacancies;

namespace Locator.Application.Vacancies;

public interface IVacanciesRepository
{
    Task<Guid> AddReviewAsync(Review review, CancellationToken cancellationToken);
    Task<Guid> SaveRatingAsync(Rating rating, CancellationToken cancellationToken);
    Task<List<Review>> GetReviewsByVacancyIdAsync(Guid vacancyId, CancellationToken cancellationToken);
    Task<int> GetDaysAfterApplyingAsync(Guid vacancyId, Guid UserId, CancellationToken cancellationToken);
}