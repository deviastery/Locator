using Ratings.Contracts.Dto;

namespace Ratings.Contracts.Responses;

public record VacancyRatingsResponse(VacancyRatingDto[]? VacancyRatings);