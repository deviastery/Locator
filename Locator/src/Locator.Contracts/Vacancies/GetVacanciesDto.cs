namespace Locator.Contracts.Vacancies;

public record GetVacanciesDto(string? Search, Guid? UserId, bool? IsApplied, int? Page, int? PageSize, int? Limit);