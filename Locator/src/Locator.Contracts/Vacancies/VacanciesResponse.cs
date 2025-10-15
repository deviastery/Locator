using System.Text.Json.Serialization;

namespace Locator.Contracts.Vacancies;

public record VacanciesResponse(
    long count, 
    IEnumerable<FullVacancyDto> vacanciesDto, 
    int page, 
    int pages, 
    int perPage);