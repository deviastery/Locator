using CSharpFunctionalExtensions;
using Locator.Application.Vacancies.GetVacanciesWithFilters;
using Locator.Domain.Vacancies;
using Shared;

namespace Locator.Application.Vacancies;

public interface IVacanciesRepository
{
    Task<Guid> CreateReviewAsync(Review review, CancellationToken cancellationToken);
    Task<List<Review>> GetReviewsByVacancyIdAsync(Guid vacancyId, CancellationToken cancellationToken);
    Task<int> GetDaysAfterApplyingAsync(Guid vacancyId, string userName, CancellationToken cancellationToken);
    Task<Result<Vacancy, Error>> GetVacancyByIdAsync(Guid vacancyId, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<Vacancy>, Error>> GetVacanciesWithFiltersAsync(GetVacanciesWithFiltersCommand command, CancellationToken cancellationToken);
}