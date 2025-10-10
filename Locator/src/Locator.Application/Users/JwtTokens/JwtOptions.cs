﻿namespace Locator.Application.Users.JwtTokens;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Secret { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string TokenValidityMins { get; init; } = default!;
    public string RefreshTokenValidityMins { get; init; } = default!;
}