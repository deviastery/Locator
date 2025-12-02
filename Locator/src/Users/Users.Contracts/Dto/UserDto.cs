using System.Text.Json.Serialization;

namespace Users.Contracts.Dto;

public record UserDto
{
    public UserDto(string? employeeId = null, string? firstName = null, string? email = null)
    {
        EmployeeId = employeeId;
        FirstName = firstName;
        Email = email;
    }
    
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    [JsonPropertyName("id")]
    public string? EmployeeId { get; init; }
}