namespace HeadHunter.Contracts.Dto;

public record GetVacanciesDto(
    string? SearchField,
    string? Experience,
    string? Employment,
    string? Schedule,
    string? Area,
    SalaryQuery? Salary,
    int? PerPage = null,
    int? Pages = null,
    int? Page = null);

public record SalaryQuery(string? Currency, int? From, int? To);
