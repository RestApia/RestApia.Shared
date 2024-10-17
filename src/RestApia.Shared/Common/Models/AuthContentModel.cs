namespace RestApia.Shared.Common.Models;

public record AuthContentModel : ValuesContentModel
{
    public required string AuthorizationServiceName { get; init; }
}
