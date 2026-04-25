namespace Shared.Options;

public class TokenCacheOptions
{
    public const string SECTION_NAME = "TokenCache";
    public string EmployeeTokenExpiryMins { get; set; } = default!;
    public string RefreshTokenExpiryMins { get; set; } = default!;
}