using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;

namespace Locator.Application.Vacancies.GetReviewsByVacancyIdQuery;

public record GetReviewsByVacancyIdQuery(long VacancyId) : IQuery;