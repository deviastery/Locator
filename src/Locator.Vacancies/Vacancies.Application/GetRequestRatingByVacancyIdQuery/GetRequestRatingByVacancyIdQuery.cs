using Shared.Abstractions;

namespace Vacancies.Application.GetRequestRatingByVacancyIdQuery;

public record GetRequestRatingByVacancyIdQuery(long VacancyId) : IQuery;