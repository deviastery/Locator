using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies.Dto;

namespace Locator.Application.Vacancies.GetVacanciesWithFiltersQuery;

public record GetVacanciesWithFiltersQuery(
    GetVacanciesByUserIdDto Dto) : IQuery;