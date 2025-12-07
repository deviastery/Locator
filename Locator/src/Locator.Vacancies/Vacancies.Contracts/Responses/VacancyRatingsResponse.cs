using Vacancies.Contracts.Dto;

namespace Vacancies.Contracts.Responses;

public record VacancyRatingsResponse(VacancyRatingDto[]? VacancyRatings);