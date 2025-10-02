using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;

namespace Locator.Application.Ratings.GetRatingByVacancyIdQuery;

public record GetRatingByVacancyIdQuery(
    GetVacancyIdDto Dto) : IQuery;