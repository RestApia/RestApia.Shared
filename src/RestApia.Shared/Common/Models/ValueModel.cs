using RestApia.Shared.Common.Enums;
namespace RestApia.Shared.Common.Models;

public record ValueModel
{
    public required string Name { get; init; }
    public required TemplatedStringModel Value { get; init; }
    public required ValueTypeEnum Type { get; init; }
    public string TypeDetails { get; init; } = string.Empty;
    public string SourceName { get; init; } = string.Empty;
}
