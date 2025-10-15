using CSharpFunctionalExtensions;
using Locator.Domain.Vacancies;
using Shared;

namespace Locator.Application.Vacancies;

public interface IVacanciesRepository
{
    Task<Guid> CreateReviewAsync(Review review, CancellationToken cancellationToken);
    Task<List<Review>> GetReviewsByVacancyIdAsync(string vacancyId, CancellationToken cancellationToken);
    Task<int> GetDaysAfterApplyingAsync(string vacancyId, string userName, CancellationToken cancellationToken);
    Task<Result<Vacancy, Error>> GetVacancyByIdAsync(string vacancyId, CancellationToken cancellationToken);
}