using Locator.Contracts.Vacancies.Dto;

namespace Locator.Contracts.Vacancies.Responses;

public record VacancyResponse(FullVacancyDto Vacancy, IEnumerable<ReviewDto>? Reviews);