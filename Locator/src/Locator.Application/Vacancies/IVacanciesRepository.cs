using CSharpFunctionalExtensions;
using Locator.Domain.Vacancies;
using Shared;

namespace Locator.Application.Vacancies;

public interface IVacanciesRepository
{
    Task<Guid> CreateReviewAsync(Review review, CancellationToken cancellationToken);
    Task<List<Review>> GetReviewsByVacancyIdAsync(Guid vacancyId, CancellationToken cancellationToken);
    Task<int> GetDaysAfterApplyingAsync(Guid vacancyId, string userName, CancellationToken cancellationToken);
    Result<bool, Error> IsVacancyReadyForReview(int daysAfterApplying);
}