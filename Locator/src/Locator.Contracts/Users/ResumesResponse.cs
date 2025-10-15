﻿using System.Text.Json.Serialization;

namespace Locator.Contracts.Users;

public record ResumesResponse
{
    [JsonPropertyName("found")]
    public int Count { get; init; }

    [JsonPropertyName("items")]
    public IEnumerable<ResumeDto>? Resumes { get; init; }

    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("pages")]
    public int Pages { get; init; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; init; }
}