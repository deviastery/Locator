using System.Text.Json.Serialization;
using HeadHunter.Contracts.Dto;

namespace Vacancies.Contracts.Responses;

public record UserResponse
{
    [JsonPropertyName("user")]
    public UserDto? User { get; init; }
}