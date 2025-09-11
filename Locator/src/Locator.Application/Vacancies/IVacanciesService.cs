using Locator.Contracts.Vacancies;

namespace Locator.Application.Vacancies;

public interface IVacanciesService
{
    Task<Guid> CreateReview(
        Guid vacancyId,
        CreateReviewDto reviewDto,
        CancellationToken cancellationToken);
}