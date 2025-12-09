using System.Text.Json.Serialization;

namespace HeadHunter.Contracts.Dto;

public record ResumeDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;
    
    [JsonPropertyName("title")]
    public string Name { get; init; } = default!;

    [JsonPropertyName("status")]
    public ResumeStatusDto? Status { get; init; }
}

public record ResumeStatusDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;
}