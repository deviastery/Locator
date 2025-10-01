using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;

namespace Locator.Application.Vacancies.GetVacancyByIdQuery;

public record GetVacancyByIdQuery(
    GetVacancyIdDto Dto) : IQuery;