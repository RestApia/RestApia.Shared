using RestApia.Shared.Common.Models;
namespace RestApia.Shared.Extensions.ImportService;

public record ImportedCollectionItem
{
    /// <summary>
    /// Type of item.
    /// </summary>
    public required ImportedCollectionItemType ItemType { get; init; }

    /// <summary>
    /// Display name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// String content (e.g. request content).
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Collection item environments.
    /// </summary>
    public IReadOnlyCollection<ValuesContentModel> Environments { get; init; } = [];

    /// <summary>
    /// Collection item values (variables, headers, cookies, etc).
    /// </summary>
    public IReadOnlyCollection<ValuesContentModel> Values { get; init; } = [];

    /// <summary>
    /// Collection item authorizations.
    /// </summary>
    public IReadOnlyCollection<AuthContentModel> Authorizations { get; init; } = [];

    /// <summary>
    /// Children collection items.
    /// </summary>
    public IReadOnlyCollection<ImportedCollectionItem> Children { get; init; } = [];
}
