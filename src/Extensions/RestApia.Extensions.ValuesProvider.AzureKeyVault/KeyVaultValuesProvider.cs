using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using RestApia.Shared.Common;
using RestApia.Shared.Common.Enums;
using RestApia.Shared.Common.Interfaces;
using RestApia.Shared.Common.Models;
using RestApia.Shared.Common.Services;
using RestApia.Shared.Extensions.ValuesProviderService.Enums;
using RestApia.Shared.Extensions.ValuesProviderService.Interfaces;
using RestApia.Shared.Extensions.ValuesProviderService.Models;
namespace RestApia.Extensions.ValuesProvider.AzureKeyVault;

[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class KeyVaultValuesProvider : IUserValuesProvider
{
    private const string ValuesLoadedDateKey = "ValuesLoadedDate";

    private static readonly ValuesProviderSettings Settings = new ()
    {
        Title = "Azure KeyVault Secrets",
        DefaultName = "Azure Secrets",
        CanBeReloaded = true,
        HelpPageUrl = "https://github.com/RestApia/RestApia.Shared/tree/main/src/Extensions/RestApia.Extensions.ValuesProvider.AzureKeyVault",
        ReservedValues =
        [
            new () { Name = nameof(KeyVaultSettings.KeyVaultUrl), Description = "KeyVault URL", IsRequired = true, Placeholder = "https://my-keyvault.vault.azure.net" },
        ],
    };

    private readonly ILogger _logger;
    private readonly IExtensionDialogs _dialogs;

    public KeyVaultValuesProvider(ILogger logger, IExtensionDialogs dialogs)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
    }

    public ValuesProviderSettings GetProviderSettings() => Settings;

    public async Task<ReloadValuesResults> ReloadValuesAsync(IReadOnlyCollection<ValueModel> inputValues, ValuesReloadMode mode)
    {
        // all templated values must be replaced
        var hasUnresolvedValues = inputValues.All(x => x.Value.Parts.All(y => !y.IsTemplatedVariable));
        if (!hasUnresolvedValues) return ReloadValuesResults.Failed;

        // validate settings
        if (!Settings.ValidateReserved(inputValues, out var errors))
        {
            var resultError = errors.Count == 1
                ? errors.ElementAt(0)
                : errors.Select(x => $"- {x}").JoinString("\r\n");
            return ReloadValuesResults.FailedWithMessage(resultError);
        }

        // try to deserialize settings
        if (!VariablesConverter.TryDeserialize<KeyVaultSettings>(inputValues, out var settings))
            return ReloadValuesResults.Failed;

        var values = await GetRemoteValuesAsync(settings);
        if (values == null) return ReloadValuesResults.Failed;

        return new ReloadValuesResults
        {
            Values = values,
            Status = ValueReloadResultType.Success,
        };
    }

    private async Task<IReadOnlyCollection<ValueModel>?> GetRemoteValuesAsync(KeyVaultSettings settings)
    {
        if (!Uri.TryCreate(settings.KeyVaultUrl, UriKind.Absolute, out var keyVaultUrl))
        {
            _dialogs.ShowError("Cannot read KeyVault secret values. KeyVault URL is not valid.");
            return null;
        }

        var credentials = BuildCredentials(settings);
        var client = BuildClient(keyVaultUrl, credentials);
        var result = await ReadValuesAsync(client, settings);

        if (result == null) return null;

        return
        [
            new ()
            {
                Name = ValuesLoadedDateKey,
                Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                Type = ValueTypeEnum.Other,
            },
            ..result,
        ];
    }

    private async Task<IReadOnlyCollection<ValueModel>?> ReadValuesAsync(SecretClient client, KeyVaultSettings settings)
    {
        try
        {
            // get list of secrets
            var secretProperties = client.GetPropertiesOfSecrets();
            var result = new List<ValueModel>();

            foreach (var secretProperty in secretProperties)
            {
                var secret = await client.GetSecretAsync(secretProperty.Name);
                result.Add(new ValueModel
                {
                    Name = secretProperty.Name,
                    Value = secret.Value.Value,
                    Type = ValueTypeEnum.Variable,
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.Fail(ex, $"Cannot read KeyVault secret values from remote. {ex.Message}");
            _dialogs.ShowError("Cannot read KeyVault secret values from remote.");

            return null;
        }
    }

    private static SecretClient BuildClient(Uri url, TokenCredential credentials)
    {
        return new SecretClient(url, credentials);
    }

    private static TokenCredential BuildCredentials(KeyVaultSettings settings)
    {
        // will try to read credentials from environment variables, managed identity, etc.
        var result = new DefaultAzureCredential();
        return result;
    }
}
