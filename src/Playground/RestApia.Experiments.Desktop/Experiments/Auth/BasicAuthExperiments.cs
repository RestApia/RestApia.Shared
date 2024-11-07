using Avalonia.Controls;
using CustomMessageBox.Avalonia;
using RestApia.Extensions.Auth.Basic;
using RestApia.Shared.Common.Enums;
using RestApia.Shared.Extensions.ValuesProviderService.Enums;
namespace RestApia.Experiments.Desktop.Experiments.Auth;

public class BasicAuthExperiments
{
    public static async Task RunBasicAsync()
    {
        var service = new BasicAuthService();
        var result = await service.ReloadValuesAsync([
            new () { Name = nameof(BasicAuthSettings.Name), Value = "User", Type = ValueTypeEnum.Variable },
            new () { Name = nameof(BasicAuthSettings.Password), Value = "Pa$$word", Type = ValueTypeEnum.Variable },
        ], ValuesReloadMode.Interactive);

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

        var message = result.Values.Select(x => $"{x.Name}: {x.Value}");
        message = [$"Expired: {result.ExpiredAt}", ..message];

        await MessageBox.Show(new SelectableTextBlock
            {
                Text = string.Join("\n", message),
            },
            "Authorization",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
