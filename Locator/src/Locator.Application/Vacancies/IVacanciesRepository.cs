using CSharpFunctionalExtensions;
using Locator.Domain.Vacancies;
using Shared;

namespace Locator.Application.Vacancies;

public interface IVacanciesRepository
{
    Task<Guid> CreateReviewAsync(Review review, CancellationToken cancellationToken);
    Task<List<Review>> GetReviewsByVacancyIdAsync(long vacancyId, CancellationToken cancellationToken);
    Task<int> GetDaysAfterApplyingAsync(long vacancyId, string userName, CancellationToken cancellationToken);
}