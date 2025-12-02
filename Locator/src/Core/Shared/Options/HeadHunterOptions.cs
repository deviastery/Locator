namespace Shared.Options;

public class HeadHunterOptions
{
    public const string SECTION_NAME = "HeadHunter";
    public string ClientId { get; init; } = default!;
    public string ClientSecret { get; init; } = default!;
    public string RedirectUri { get; init; } = default!;
    public string Scope { get; init; } = default!;
}