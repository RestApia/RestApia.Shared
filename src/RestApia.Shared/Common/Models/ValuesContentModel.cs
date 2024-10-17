namespace RestApia.Shared.Common.Models;

public record ValuesContentModel
{
    public required string Name { get; init; }
    public string Content { get; init; } = string.Empty;
}
