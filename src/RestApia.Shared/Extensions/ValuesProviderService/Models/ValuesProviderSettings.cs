namespace RestApia.Shared.Extensions.ValuesProviderService.Models;

public record ValuesProviderSettings
{
    public required string Title { get; init; }
    public required string DefaultName { get; init; } = string.Empty;

    public bool CanBeReloaded { get; init; }
    public bool DisableCachingResults { get; init; }
    public string HelpPageUrl { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public IReadOnlyCollection<ReservedValueInfo> ReservedValues { get; init; } = [];
    public IReadOnlyDictionary<string, string> Examples { get; init; } = new Dictionary<string, string>();
}
