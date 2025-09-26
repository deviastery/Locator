namespace Locator.Application.Vacancies.GetVacanciesWithFilters;

public record VacancyDto(
    Guid Id,
    string Name,
    string Description,
    int? Salary,
    int? Experience,
    double? Rating);