namespace RestApia.Shared.Common.Models;

public record AuthContentModel : ValuesContentModel
{
    public required string ProviderClassFullName { get; init; }
}
