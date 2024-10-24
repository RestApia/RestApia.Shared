﻿namespace RestApia.Shared.Extensions.ImportService;

/// <summary>
/// Collection import service.
/// </summary>
public interface IImportService
{
    /// <summary>
    /// Display name in UI.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Import collection items from file.
    /// </summary>
    /// <param name="path">Full path to file.</param>
    ImportedFileModel Import(string path);
}
