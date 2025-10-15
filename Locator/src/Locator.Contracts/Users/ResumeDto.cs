using System.Text.Json.Serialization;

namespace Locator.Contracts.Users;

public record ResumeDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("status")]
    public ResumeStatusDto? Status { get; init; }
}

public record ResumeStatusDto
{
    [JsonPropertyName("id")]
    public ResumeStatusEnum Enum { get; init; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;
}

public enum ResumeStatusEnum
{
    /// <summary>
    /// Type Not Published resume
    /// </summary>
    NOT_PUBLISHED,
    
    /// <summary>
    /// Type Published resume
    /// </summary>
    PUBLISHED,
    
    /// <summary>
    /// Type Blocked resume
    /// </summary>
    BLOCKED,
    
    /// <summary>
    /// Type On Moderation resume
    /// </summary>
    ON_MODERATION,
}