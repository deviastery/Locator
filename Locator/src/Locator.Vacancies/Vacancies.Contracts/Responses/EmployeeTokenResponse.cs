using System.Text.Json.Serialization;
using HeadHunter.Contracts.Dto;

namespace Vacancies.Contracts.Responses;

public record EmployeeTokenResponse
{
    [JsonPropertyName("employeeToken")]
    public EmployeeTokenDto? EmployeeToken { get; init; }
}