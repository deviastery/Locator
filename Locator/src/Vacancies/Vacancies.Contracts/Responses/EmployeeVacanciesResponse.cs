using System.Text.Json.Serialization;
using Vacancies.Contracts.Dto;

namespace Vacancies.Contracts.Responses;

public record EmployeeVacanciesResponse
{
    [JsonPropertyName("found")]
    public long Count { get; init; }

    [JsonPropertyName("items")]
    public List<VacancyDto>? Vacancies { get; init; }

    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("pages")]
    public int Pages { get; init; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; init; }
}