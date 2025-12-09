using Ratings.Contracts.Dto;
using Shared.Abstractions;

namespace Ratings.Application.GetRatingByVacancyIdQuery;

public record GetRatingByVacancyIdQuery(GetRatingByVacancyIdDto Dto) : IQuery;