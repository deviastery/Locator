namespace Shared.Options;

public class HhApiOptions
{
    public const string SECTION_NAME = "HhApi";
    public string ClientId { get; init; } = default!;
    public string ClientSecret { get; init; } = default!;
    public string RedirectUri { get; init; } = default!;
    public string Scope { get; init; } = default!;
}