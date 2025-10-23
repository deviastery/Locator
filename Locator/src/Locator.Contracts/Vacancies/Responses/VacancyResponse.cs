using Locator.Contracts.Vacancies.Dtos;

namespace Locator.Contracts.Vacancies.Responses;

public record VacancyResponse(FullVacancyDto Vacancy, IEnumerable<ReviewDto>? Reviews);