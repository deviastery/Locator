using Shared.Abstractions;

namespace Vacancies.Application.GetReviewsByVacancyIdQuery;

public record GetReviewsByVacancyIdQuery(long VacancyId) : IQuery;