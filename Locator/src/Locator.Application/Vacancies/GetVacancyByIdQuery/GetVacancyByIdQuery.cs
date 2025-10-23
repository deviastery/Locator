using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;
using Locator.Contracts.Vacancies.Dtos;

namespace Locator.Application.Vacancies.GetVacancyByIdQuery;

public record GetVacancyByIdQuery(
    GetVacancyIdDto Dto) : IQuery;