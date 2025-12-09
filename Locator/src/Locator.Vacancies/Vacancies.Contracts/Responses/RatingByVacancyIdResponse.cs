using System.Text.Json.Serialization;
using Vacancies.Contracts.Dto;

namespace Vacancies.Contracts.Responses;

public record RatingByVacancyIdResponse
{
    [JsonPropertyName("rating")]
    public VacancyRatingDto? Rating { get; init; }
}