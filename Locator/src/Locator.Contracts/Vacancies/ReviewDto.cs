namespace Locator.Contracts.Vacancies;

public record ReviewDto(
    Guid Id,
    double Mark,
    string? Comment,
    string UserName);