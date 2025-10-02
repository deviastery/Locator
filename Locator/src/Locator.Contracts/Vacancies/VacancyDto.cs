namespace Locator.Contracts.Vacancies;

public record VacancyDto(
    Guid Id,
    string Name,
    string Description,
    int? Salary,
    int? Experience,
    double? Rating);