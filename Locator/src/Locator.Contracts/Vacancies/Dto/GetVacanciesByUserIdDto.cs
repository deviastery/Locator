namespace Locator.Contracts.Vacancies.Dto;

public record GetVacanciesByUserIdDto(Guid UserId, GetVacanciesDto Query);