using System.Text.Json.Serialization;
using Vacancies.Contracts.Dto;

namespace Vacancies.Contracts.Responses;

public record VacancyRatingsResponse
{
    [JsonPropertyName("vacancyRatings")]
    public VacancyRatingDto[]? VacancyRatings { get; init; }
}