using System.Text.Json.Serialization;
using HeadHunter.Contracts.Dto;

namespace HeadHunter.Contracts.Responses;

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