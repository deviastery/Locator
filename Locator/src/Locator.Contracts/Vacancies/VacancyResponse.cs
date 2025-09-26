namespace Locator.Application.Vacancies.GetVacanciesWithFilters;

public record VacancyResponse(IEnumerable<VacancyDto> Vacancies, int TotalCount);