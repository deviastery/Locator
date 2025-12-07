namespace Vacancies.Contracts.Dto;

public record VacancyRatingDto(
    Guid Id,
    double Value,
    long EntityId);