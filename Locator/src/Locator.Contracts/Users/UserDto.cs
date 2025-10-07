using System.Text.Json.Serialization;

namespace Locator.Contracts.Users;

public record UserDto
{
    [JsonPropertyName("auth_type")]
    public string AuthType { get; init; } = default!;

    [JsonPropertyName("is_applicant")]
    public bool IsApplicant { get; init; }

    [JsonPropertyName("is_employer")]
    public bool IsEmployer { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    [JsonPropertyName("middle_name")]
    public string? MiddleName { get; init; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }

    [JsonPropertyName("id")]
    public string? EmployeeId { get; init; }

    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    public string FullName => 
        string.Join(" ", new[] { FirstName, MiddleName, LastName }
            .Where(x => !string.IsNullOrWhiteSpace(x)));
}