using CustomMessageBox.Avalonia;
using RestApia.Experiments.Desktop.Modules.Dialogs;
using RestApia.Experiments.Desktop.Modules.Logger;
using RestApia.Extensions.Auth.OAuth2.AuthCode;
using RestApia.Extensions.Auth.OAuth2.Implicit;
using RestApia.Shared.Common.Enums;
using RestApia.Shared.Common.Models;
using RestApia.Shared.Common.Services;
using RestApia.Shared.Extensions.ValuesProviderService.Enums;
namespace RestApia.Experiments.Desktop.Experiments.Auth;

public static class OAuth2Experiments
{
    public static async Task RunAuthCodeAsync()
    {
        var service = new OAuth2AuthCodeService(ConsoleExtensionLogger.Instance, new ExtensionDialogs());
        var settings = LocalSettings.Get<OAuth2AuthCodeSettings>();
        var result = await service.ReloadValuesAsync(VariablesConverter.Serialize(settings), ValuesReloadMode.Interactive);

        if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
        {
            await MessageBox.Show(result.ErrorMessage, "Authorization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (result.Status != ValueReloadResultType.Success)
        {
            await MessageBox.Show($"Authorization status: {result.Status}", "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await MessageGrid.ShowAsync(result
            .Values
            .Prepend(new ValueModel { Name = "Expired At", Value = result.ExpiredAt?.ToString() ?? "Never expired", Type = ValueTypeEnum.Other })
            .Select(x => new { x.Name, Value = x.Value.ToString() }));
    }

    public static async Task RunImplicitAsync()
    {
        var service = new OAuth2ImplicitService(ConsoleExtensionLogger.Instance, new ExtensionDialogs());
        var settings = LocalSettings.Get<OAuth2ImplicitSettings>();
        var result = await service.ReloadValuesAsync(VariablesConverter.Serialize(settings), ValuesReloadMode.Interactive);

        if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
        {
            await MessageBox.Show(result.ErrorMessage, "Authorization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (result.Status != ValueReloadResultType.Success)
        {
            await MessageBox.Show($"Authorization status: {result.Status}", "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await MessageGrid.ShowAsync(result
            .Values
            .Prepend(new ValueModel { Name = "Expired At", Value = result.ExpiredAt?.ToString() ?? "Never expired", Type = ValueTypeEnum.Other })
            .Select(x => new { x.Name, Value = x.Value.ToString() }));
    }
}
