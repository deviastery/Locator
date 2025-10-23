using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;
using Locator.Contracts.Vacancies.Dtos;

namespace Locator.Application.Ratings.GetRatingByVacancyIdQuery;

public record GetRatingByVacancyIdQuery(
    GetVacancyIdDto Dto) : IQuery;