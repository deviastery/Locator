using HeadHunter.Contracts.Dto;

namespace Vacancies.Contracts.Dto;

public record GetVacanciesByUserIdDto(Guid UserId, GetVacanciesDto Query);