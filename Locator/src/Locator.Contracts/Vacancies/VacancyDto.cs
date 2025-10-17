using System.Text.Json.Serialization;

namespace Locator.Contracts.Vacancies;

public record FullVacancyDto
{
    public FullVacancyDto(long id, VacancyDto dto, double? rating)
    {
        Id = id;
        Name = dto.Name;
        Description = dto.Description.Responsibility;
        Employer = dto.Employer.Name;
        Area = dto.Area;
        Address = dto.Address;
        Experience = dto.Experience;
        Salary = dto.Salary;
        Schedule = dto.Schedule;
        WorkFormat = dto.WorkFormat;
        Rating = rating;
    }
    
    public long Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Employer { get; init; }
    public Area Area { get; init; }
    public Address? Address { get; init; }
    public Experience? Experience { get; init; }
    public Salary? Salary { get; init; }
    public Schedule? Schedule { get; init; }
    public List<WorkFormat>? WorkFormat { get; init; }
    public double? Rating { get; init; }
}

public record VacancyDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;
    
    [JsonPropertyName("snippet")]
    public Snippet Description { get; init; } = default!;
    
    [JsonPropertyName("employer")]
    public Employer Employer { get; init; } = default!;
    
    [JsonPropertyName("area")]
    public Area Area { get; init; } = default!;

    [JsonPropertyName("address")]
    public Address? Address { get; init; } = default!;
    
    [JsonPropertyName("experience")]
    public Experience? Experience { get; init; } = default!;
    
    [JsonPropertyName("salary")]
    public Salary? Salary { get; init; } = default!;
    
    [JsonPropertyName("schedule")]
    public Schedule? Schedule { get; init; } = default!;
    
    [JsonPropertyName("work_format")]
    public List<WorkFormat>? WorkFormat { get; init; } = default!;
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

public enum ScheduleEnum
{
    FULL_DAY,
    SHIFT,
    FLEXIBLE,
    REMOTE,
    FLY_IN_FLY_OUT,
}

public enum WorkFormatEnum
{
    ON_SITE,
    REMOTE,
    HYBRID,
    FIELD_WORK,
    FLY_IN_FLY_OUT,
}