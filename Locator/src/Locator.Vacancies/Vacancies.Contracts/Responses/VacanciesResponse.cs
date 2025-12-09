using Vacancies.Contracts.Dto;

namespace Vacancies.Contracts.Responses;

public record VacanciesResponse(
    long Count, 
    IEnumerable<FullVacancyDto> Vacancies, 
    int Page, 
    int Pages, 
    int PerPage);