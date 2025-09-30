using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;

namespace Locator.Application.Vacancies.GetVacanciesWithFiltersQuery;

public record GetVacanciesWithFiltersQuery(
    GetVacanciesDto Dto) : IQuery;