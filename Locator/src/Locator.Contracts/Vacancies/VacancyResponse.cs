namespace Locator.Contracts.Vacancies;

public record VacancyResponse(FullVacancyDto Vacancy, IEnumerable<ReviewDto>? Reviews);