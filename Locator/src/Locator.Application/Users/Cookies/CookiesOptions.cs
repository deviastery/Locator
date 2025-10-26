namespace Locator.Application.Users.Cookies;

public class CookiesOptions
{
    public const string SECTION_NAME = "Cookies";
    public string JwtName { get; init; } = default!;
    public string UserName { get; init; } = default!;
}