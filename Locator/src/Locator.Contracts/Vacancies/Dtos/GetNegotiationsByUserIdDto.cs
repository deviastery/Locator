namespace Locator.Contracts.Vacancies.Dtos;

public record GetNegotiationsByUserIdDto(Guid UserId, GetNegotiationsDto Query);