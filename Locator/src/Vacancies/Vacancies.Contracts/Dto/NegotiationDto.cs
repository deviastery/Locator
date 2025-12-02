using System.Text.Json.Serialization;

namespace Vacancies.Contracts.Dto;

public record NegotiationDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;
    
    [JsonPropertyName("state")]
    public State State { get; init; } = default!;    
    
    [JsonPropertyName("viewed_by_opponent")]
    public bool ViewedByOpponent { get; init; } = default!;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; init; } = default!;
    
    [JsonPropertyName("vacancy")]
    public VacancyDto Vacancy { get; init; } = default!;
}

public record State
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;
}
