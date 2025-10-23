using Locator.Contracts.Vacancies.Dtos;

namespace Locator.Contracts.Vacancies.Responses;

public record VacanciesResponse(
    long Count, 
    IEnumerable<FullVacancyDto> Vacancies, 
    int Page, 
    int Pages, 
    int PerPage);