using CustomMessageBox.Avalonia;
using RestApia.Experiments.Desktop.Modules.Auth;
using RestApia.Shared.Common.Interfaces;
using RestApia.Shared.Common.Models;
namespace RestApia.Experiments.Desktop.Modules.Dialogs;

public class ExtensionDialogs : IExtensionDialogs
{
    public void ShowError(string message) => MessageBox.Show(message, "Error -_-", MessageBoxButtons.OK, MessageBoxIcon.Error);

    public Task<BrowserDialogResult> OpenAuthBrowserAsync(string url, string stopUrl, string title)
    {
        var dialog = new AuthWindow(url, stopUrl);
        return dialog.ShowDialog<BrowserDialogResult>(App.AppWindow);
    }
}
