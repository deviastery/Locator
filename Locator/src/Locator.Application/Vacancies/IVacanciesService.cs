using CSharpFunctionalExtensions;
using Locator.Contracts.Vacancies;
using Shared;

namespace Locator.Application.Vacancies;

public interface IVacanciesService
{
    Task<Result<Guid, Failure>> CreateReview(
        Guid vacancyId,
        CreateReviewDto reviewDto,
        CancellationToken cancellationToken);
}