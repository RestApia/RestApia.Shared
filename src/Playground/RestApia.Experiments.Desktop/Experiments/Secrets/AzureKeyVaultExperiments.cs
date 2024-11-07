using CustomMessageBox.Avalonia;
using RestApia.Experiments.Desktop.Modules.Dialogs;
using RestApia.Experiments.Desktop.Modules.Logger;
using RestApia.Extensions.ValuesProvider.AzureKeyVault;
using RestApia.Shared.Common.Services;
using RestApia.Shared.Extensions.ValuesProviderService.Enums;
namespace RestApia.Experiments.Desktop.Experiments.Secrets;

public static class AzureKeyVaultExperiments
{
    public static async Task RunAsync()
    {
        var settings = LocalSettings.Get<KeyVaultSettings>();
        var service = new KeyVaultValuesProvider(ConsoleExtensionLogger.Instance, new ExtensionDialogs());
        var result = await service.ReloadValuesAsync(VariablesConverter.Serialize(settings), ValuesReloadMode.Interactive);

        if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
        {
            await MessageBox.Show(result.ErrorMessage, "Secrets reading Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (result.Status != ValueReloadResultType.Success)
        {
            await MessageBox.Show($"Secrets status: {result.Status}", "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await MessageGrid.ShowAsync(result
            .Values
            .Select(x => new { x.Name, Value = x.Value.ToString() }));
    }
}
