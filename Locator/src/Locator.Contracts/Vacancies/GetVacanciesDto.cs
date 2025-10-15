namespace Locator.Contracts.Vacancies;

public record GetVacanciesDto(string? Search, bool? IsApplied, int? Page, int? PageSize, int? Limit);