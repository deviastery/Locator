namespace Locator.Infrastructure.HhApi.Users;

public class HhApiConfiguration
{
    public const string SectionName = "HhApi";

    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string RedirectUri { get; set; } = default!;
}