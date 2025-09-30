namespace Locator.Contracts.Vacancies;

public record VacancyResponse(IEnumerable<VacancyDto> Vacancies, long TotalCount);