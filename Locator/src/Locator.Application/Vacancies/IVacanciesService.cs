using Locator.Contracts.Vacancies;
using Locator.Domain.Vacancies;

namespace Locator.Application.Vacancies;

public interface IVacanciesService
{
    Task<Guid> AddReview(
        Guid vacancyId,
        AddReviewDto reviewDto,
        CancellationToken cancellationToken);
}