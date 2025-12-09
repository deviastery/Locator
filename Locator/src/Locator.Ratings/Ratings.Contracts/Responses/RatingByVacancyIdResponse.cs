using Ratings.Contracts.Dto;

namespace Ratings.Contracts.Responses;

public record RatingByVacancyIdResponse(VacancyRatingDto? Rating);