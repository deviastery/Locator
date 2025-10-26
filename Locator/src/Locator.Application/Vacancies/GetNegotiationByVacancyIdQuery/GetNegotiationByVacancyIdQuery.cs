using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies.Dto;

namespace Locator.Application.Vacancies.GetNegotiationByVacancyIdQuery;

public record GetNegotiationByVacancyIdQuery(
    GetNegotiationByVacancyIdDto Dto) : IQuery;