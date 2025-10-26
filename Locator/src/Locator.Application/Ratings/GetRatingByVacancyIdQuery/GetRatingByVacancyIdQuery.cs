using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies.Dto;

namespace Locator.Application.Ratings.GetRatingByVacancyIdQuery;

public record GetRatingByVacancyIdQuery(GetVacancyIdDto Dto) : IQuery;