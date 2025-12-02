using Shared.Abstractions;
using Vacancies.Contracts.Dto;

namespace Ratings.Application.GetRatingByVacancyIdQuery;

public record GetRatingByVacancyIdQuery(GetVacancyIdDto Dto) : IQuery;