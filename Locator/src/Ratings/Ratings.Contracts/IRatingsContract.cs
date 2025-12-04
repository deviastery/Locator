using CSharpFunctionalExtensions;
using Ratings.Contracts.Dto;
using Shared;
using Vacancies.Contracts.Dto;

namespace Ratings.Contracts;

public interface IRatingsContract
{
    /// <summary>
    /// Gets a Vacancy rating
    /// </summary>
    /// <param name="dto">Dto for getting Vacancy rating</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dto of Vacancy Rating</returns>
    Task<VacancyRatingDto?> GetRatingDtoByVacancyIdAsync(GetRatingByVacancyIdDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a Vacancy ratings
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of Dto of Vacancy ratings</returns>
    Task<VacancyRatingDto[]> GetVacancyRatingsDtoAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Updates a Vacancy rating
    /// </summary>
    /// <param name="updateVacancyRatingDto">Dto for updating Vacancy rating</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Guid of new Vacancy rating</returns>
    Task<Result<Guid, Failure>> UpdateVacancyRatingAsync(
        UpdateVacancyRatingDto updateVacancyRatingDto,
        CancellationToken cancellationToken);
}