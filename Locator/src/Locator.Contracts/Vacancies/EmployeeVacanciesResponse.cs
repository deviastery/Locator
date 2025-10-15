using System.Text.Json.Serialization;

namespace Locator.Contracts.Vacancies;

public record EmployeeVacanciesResponse
{
    [JsonPropertyName("found")]
    public long Count { get; init; }

    [JsonPropertyName("items")]
    public IEnumerable<VacancyDto>? Vacancies { get; init; }

    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("pages")]
    public int Pages { get; init; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; init; }
}