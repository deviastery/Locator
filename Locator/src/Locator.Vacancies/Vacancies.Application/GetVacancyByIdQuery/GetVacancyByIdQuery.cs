using Shared.Abstractions;
using Vacancies.Contracts.Dto;

namespace Vacancies.Application.GetVacancyByIdQuery;

public record GetVacancyByIdQuery(GetVacancyIdDto Dto) : IQuery;