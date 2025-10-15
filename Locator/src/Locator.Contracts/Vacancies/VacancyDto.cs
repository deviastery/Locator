using System.Text.Json.Serialization;

namespace Locator.Contracts.Vacancies;

public record FullVacancyDto
{
    public Guid Id = Guid.NewGuid();
    public string VacancyId;
    public string Name;
    public string Description;
    public string Employer;
    public Address? Address;
    public string? Experience;
    public Salary? Salary;
    public ScheduleEnum? Schedule;
    public WorkFormatEnum? WorkFormat;
    
    public double? Rating;
    
    public FullVacancyDto(VacancyDto dto, double? rating)
    {
        VacancyId = dto.Id;
        Name = dto.Name;
        Description = dto.Description;
        Employer = dto.Employer.Id;
        Address = dto.Address;
        Experience = dto.Experience.Id;
        Salary = dto.Salary;
        Schedule = dto.Schedule.UId;
        WorkFormat = dto.WorkFormat.Id;
        Rating = rating;
    }
}

public record VacancyDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;
    
    [JsonPropertyName("description")]
    public string Description { get; init; } = default!;
    
    [JsonPropertyName("employer")]
    public Employer Employer { get; init; } = default!;

    [JsonPropertyName("address")]
    public Address Address { get; init; } = default!;
    
    [JsonPropertyName("experience")]
    public Experience Experience { get; init; } = default!;
    
    [JsonPropertyName("salary")]
    public Salary Salary { get; init; } = default!;
    
    [JsonPropertyName("schedule")]
    public Schedule Schedule { get; init; } = default!;
    
    [JsonPropertyName("work_format")]
    public WorkFormat WorkFormat { get; init; } = default!;
}

public record Employer
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;
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
    public string? StationId { get; init; }

    [JsonPropertyName("station_name")]
    public string? StationName { get; init; }
}
public record Experience
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
public record Salary
{
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("from")]
    public string? From { get; init; }
    
    [JsonPropertyName("gross")]
    public string? Gross { get; init; }

    [JsonPropertyName("to")]
    public string? To { get; init; }
}
public record Schedule
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("uid")]
    public ScheduleEnum? UId { get; init; }
}
public record WorkFormat
{
    [JsonPropertyName("id")]
    public WorkFormatEnum? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
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