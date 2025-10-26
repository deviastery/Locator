namespace Locator.Contracts.Vacancies.Dto;

public record ReviewDto(
    Guid Id,
    double Mark,
    string? Comment,
    string UserName);