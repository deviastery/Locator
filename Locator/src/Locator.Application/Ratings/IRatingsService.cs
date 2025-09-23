using CSharpFunctionalExtensions;
using Locator.Contracts.Ratings;
using Shared;

namespace Locator.Application.Ratings;

public interface IRatingsService
{
    Task<Result<Guid, Failure>> CreateVacancyRating(
        CreateVacancyRatingDto vacancyRatingDto,
        CancellationToken cancellationToken);
}