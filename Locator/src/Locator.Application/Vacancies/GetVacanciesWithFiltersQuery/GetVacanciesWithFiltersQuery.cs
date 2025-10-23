using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;
using Locator.Contracts.Vacancies.Dtos;

namespace Locator.Application.Vacancies.GetVacanciesWithFiltersQuery;

public record GetVacanciesWithFiltersQuery(
    GetVacanciesByUserIdDto Dto) : IQuery;