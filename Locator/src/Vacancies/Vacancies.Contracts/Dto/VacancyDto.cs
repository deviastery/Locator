using System.Text.Json.Serialization;

namespace Vacancies.Contracts.Dto;

public record VacancyDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;
    
    [JsonPropertyName("alternate_url")]
    public string Url { get; init; } = default!;
    
    [JsonPropertyName("snippet")]
    public Snippet? Description { get; init; }
    
    [JsonPropertyName("employer")]
    public Employer? Employer { get; init; }
    
    [JsonPropertyName("area")]
    public Area? Area { get; init; }

    [JsonPropertyName("address")]
    public Address? Address { get; init; }
    
    [JsonPropertyName("experience")]
    public Experience? Experience { get; init; }
    
    [JsonPropertyName("salary")]
    public Salary? Salary { get; init; }
    
    [JsonPropertyName("schedule")]
    public Schedule? Schedule { get; init; }
    
    [JsonPropertyName("work_format")]
    public List<WorkFormat>? WorkFormat { get; init; }
}

public record Snippet
{
    [JsonPropertyName("requirement")]
    public string Requirement { get; init; } = default!;

    [JsonPropertyName("responsibility")]
    public string Responsibility { get; init; } = default!;
}
public record Employer
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;
}
public record Area
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("name")]
    public string City { get; init; } = default!;
}

public record Address
{
    [JsonPropertyName("city")]
    public string? City { get; init; }

    [JsonPropertyName("metro_stations")]
    public MetroStation[]? MetroStations { get; init; }
}

public record MetroStation
{
    [JsonPropertyName("station_id")]
    public string StationId { get; init; }

    [JsonPropertyName("station_name")]
    public string StationName { get; init; }
}
public record Experience
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }
}
public record Salary
{
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("from")]
    public int? From { get; init; }
    
    [JsonPropertyName("gross")]
    public bool? Gross { get; init; }

    [JsonPropertyName("to")]
    public int? To { get; init; }
}
public record Schedule
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("uid")]
    public string UId { get; init; }
}
public record WorkFormat
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }
}