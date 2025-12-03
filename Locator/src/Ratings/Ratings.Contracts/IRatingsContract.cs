using CSharpFunctionalExtensions;
using Ratings.Contracts.Dto;
using Shared;

namespace Ratings.Contracts;

public interface IRatingsContract
{
    /// <summary>
    /// Gets a Vacancy Rating
    /// </summary>
    /// <param name="vacancyId">ID of Vacancy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dto of Vacancy Rating</returns>
    Task<Result<VacancyRatingDto, Error>> GetRatingDtoByVacancyIdAsync(
        long vacancyId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a Vacancy Ratings
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of Dto of Vacancy Ratings</returns>
    Task<Result<VacancyRatingDto[], Error>> GetRatingsDtoAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// Updates a Vacancy Rating
    /// </summary>
    /// <param name="updateVacancyRatingDto">Dto for updating Vacancy Rating</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Guid of new Vacancy Rating</returns>
    Task<Result<Guid, Failure>> UpdateVacancyRatingAsync(
        UpdateVacancyRatingDto updateVacancyRatingDto,
        CancellationToken cancellationToken);
}