namespace Locator.Contracts.Vacancies.Dto;

public record GetNegotiationsByUserIdDto(Guid UserId, GetNegotiationsDto Query);