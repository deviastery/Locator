namespace Vacancies.Contracts.Dto;

public record ReviewDto(
    Guid Id,
    double Mark,
    string? Comment,
    string UserName);