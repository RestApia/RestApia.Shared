namespace RestApia.Shared.Extensions.ValuesProviderService.Models;

public record ReservedValueInfo
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public bool IsRequired { get; init; }
    public string Placeholder { get; set; } = string.Empty;
    public IReadOnlyCollection<string?> ExpectedValues { get; set; } = [];
}
