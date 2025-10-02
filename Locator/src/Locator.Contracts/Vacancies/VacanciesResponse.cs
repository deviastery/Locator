namespace Locator.Contracts.Vacancies;

public record VacanciesResponse(IEnumerable<VacancyDto> Vacancies, long TotalCount);