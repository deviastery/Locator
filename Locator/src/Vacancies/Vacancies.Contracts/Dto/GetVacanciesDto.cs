namespace Vacancies.Contracts.Dto;

public record GetVacanciesDto(
    string? SearchField,
    string? Experience,
    string? Employment,
    string? Schedule,
    int? Area,
    SalaryQuery? Salary,
    int? PerPage = null,
    int? Pages = null,
    int? Page = null);

public record SalaryQuery(long? Salary, string? Currency);
