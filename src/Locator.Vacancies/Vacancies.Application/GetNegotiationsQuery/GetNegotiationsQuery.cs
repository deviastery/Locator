using Shared.Abstractions;
using Vacancies.Contracts.Dto;

namespace Vacancies.Application.GetNegotiationsQuery;

public record GetNegotiationsQuery(
    GetNegotiationsByUserIdDto Dto) : IQuery;