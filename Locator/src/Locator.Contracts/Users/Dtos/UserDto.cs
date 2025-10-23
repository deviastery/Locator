using System.Text.Json.Serialization;

namespace Locator.Contracts.Users.Dtos;

public record UserDto
{
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    [JsonPropertyName("id")]
    public string? EmployeeId { get; init; }

}