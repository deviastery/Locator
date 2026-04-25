namespace Shared.Options;

public class ModeratorOptions
{
    public const string SECTION_NAME = "ModeratorLevels";
    public string Insult { get; init; } = default!;
    public string Threat { get; init; } = default!;
    public string Obscenity { get; init; } = default!;
    public string Meaningless { get; init; } = default!;
}
