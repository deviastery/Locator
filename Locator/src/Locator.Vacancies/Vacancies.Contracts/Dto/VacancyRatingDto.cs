using System.Text.Json.Serialization;

namespace Vacancies.Contracts.Dto;

public record VacancyRatingDto
{
    public VacancyRatingDto(
        Guid id,
        double value,
        long entityId)
    {
        Id = id;
        Value = value;
        EntityId = entityId;
    }
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("value")]
    public double Value { get; set; }
    [JsonPropertyName("entityId")]
    public long EntityId { get; set; }
}