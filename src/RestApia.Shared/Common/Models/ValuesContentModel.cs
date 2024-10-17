namespace RestApia.Shared.Common.Models;

/// <summary>
/// Values content. It can be Values with variables, environments, auth settings.
/// </summary>
public record ValuesContentModel
{
    /// <summary>
    /// Display name of the values settings.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// String content of the values.
    /// </summary>
    public string Content { get; init; } = string.Empty;
}
