using RestApia.Shared.Common.Models;
namespace RestApia.Shared.Common.Interfaces;

/// <summary>
/// Display application dialogs.
/// </summary>
public interface IExtensionDialogs
{
    /// <summary>
    /// Show error message to user.
    /// </summary>
    void ShowError(string message);

    /// <summary>
    /// Open web browser dialog for Authentication.
    /// </summary>
    /// <param name="url">Open Url.</param>
    /// <param name="stopUrl">When stop URL navigated, dialog will close and return results.</param>
    /// <param name="title">Window title.</param>
    Task<BrowserDialogResult> OpenAuthBrowserAsync(string url, string stopUrl, string title);
}
