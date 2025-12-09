namespace Shared.Options;

public class TokenCacheOptions
{
    public const string SECTION_NAME = "TokenCache";
    public string EmployeeTokenExpiryMins { get; init; } = default!;
    public string RefreshTokenExpiryMins { get; init; } = default!;
}