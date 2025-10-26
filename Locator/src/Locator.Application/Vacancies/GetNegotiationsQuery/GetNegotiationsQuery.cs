using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;
using Locator.Contracts.Vacancies.Dto;

namespace Locator.Application.Vacancies.GetNegotiationsQuery;

public record GetNegotiationsQuery(
    GetNegotiationsByUserIdDto Dto) : IQuery;