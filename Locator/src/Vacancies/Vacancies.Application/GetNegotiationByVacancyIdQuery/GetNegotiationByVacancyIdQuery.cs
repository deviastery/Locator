using Shared.Abstractions;
using Vacancies.Contracts.Dto;

namespace Vacancies.Application.GetNegotiationByVacancyIdQuery;

public record GetNegotiationByVacancyIdQuery(
    GetNegotiationByVacancyIdDto Dto) : IQuery;