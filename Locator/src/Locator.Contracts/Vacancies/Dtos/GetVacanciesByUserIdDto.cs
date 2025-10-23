namespace Locator.Contracts.Vacancies.Dtos;

public record GetVacanciesByUserIdDto(Guid UserId, GetVacanciesDto Query);