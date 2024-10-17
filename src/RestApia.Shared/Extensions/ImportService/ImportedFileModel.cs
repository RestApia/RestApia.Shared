using RestApia.Shared.Common.Models;
namespace RestApia.Shared.Extensions.ImportService;

/// <summary>
/// Collection import details.
/// </summary>
public record ImportedFileModel
{
    /// <summary>
    /// Full path to file.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// Display name of the file. Can be parsed colletion name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Import file type. To show in UI the icon.
    /// </summary>
    public ImportFileType Type { get; init; } = ImportFileType.General;

    /// <summary>
    /// List of environments for top level workspace.
    /// </summary>
    public IReadOnlyCollection<ValuesContentModel> WorkspaceEnvironments { get; init; } = [];

    /// <summary>
    /// List of variables for top level workspace.
    /// </summary>
    public IReadOnlyCollection<ValuesContentModel> WorkspaceVariables { get; init; } = [];

    /// <summary>
    /// List of auth settings for top level workspace.
    /// </summary>
    public IReadOnlyCollection<AuthContentModel> WorkspaceAuth { get; init; } = [];

    /// <summary>
    /// Collection items.
    /// </summary>
    public IReadOnlyCollection<ImportedCollectionItem> WorkspaceItems { get; init; } = [];
}
