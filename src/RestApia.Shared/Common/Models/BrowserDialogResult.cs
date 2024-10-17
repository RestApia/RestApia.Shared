namespace RestApia.Shared.Common.Models;

/// <summary>
/// Web browser dialog result.
/// </summary>
public record BrowserDialogResult
{
    /// <summary>
    /// Last URL.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// Headers of the last response.
    /// </summary>
    public Dictionary<string, string> Headers { get; init; } = [];
}
