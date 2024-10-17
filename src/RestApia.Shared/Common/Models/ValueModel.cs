using RestApia.Shared.Common.Enums;
namespace RestApia.Shared.Common.Models;

/// <summary>
/// Value details.
/// </summary>
public record ValueModel
{
    /// <summary>
    /// Name of value object.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Content of value object.
    /// </summary>
    public required string Value { get; init; }

    /// <summary>
    /// Value type.
    /// </summary>
    public required ValuesContentItemTypeEnum Type { get; init; }

    /// <summary>
    /// Optional description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
