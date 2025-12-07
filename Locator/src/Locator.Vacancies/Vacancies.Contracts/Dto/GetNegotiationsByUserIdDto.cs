using HeadHunter.Contracts.Dto;

namespace Vacancies.Contracts.Dto;

public record GetNegotiationsByUserIdDto(Guid UserId, GetNegotiationsDto Query);