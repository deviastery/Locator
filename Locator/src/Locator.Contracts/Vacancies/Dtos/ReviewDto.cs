namespace Locator.Contracts.Vacancies.Dtos;

public record ReviewDto(
    Guid Id,
    double Mark,
    string? Comment,
    string UserName);