using Shared.Abstractions;
using Vacancies.Contracts.Dto;

namespace Vacancies.Application.GetVacanciesWithFiltersQuery;

public record GetVacanciesWithFiltersQuery(
    GetVacanciesByUserIdDto Dto) : IQuery;