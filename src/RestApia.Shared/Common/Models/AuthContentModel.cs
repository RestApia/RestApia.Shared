namespace RestApia.Shared.Common.Models;

public record AuthContentModel : ValuesContentModel
{
    public required string SettingsClassFullName { get; init; }
}
