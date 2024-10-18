using RestApia.Shared.Common.Models;
using RestApia.Shared.Extensions.ValuesProviderService.Enums;
using RestApia.Shared.Extensions.ValuesProviderService.Models;
namespace RestApia.Shared.Extensions.ValuesProviderService.Interfaces;

/// <summary>
/// Values provider. Add variables, headers and cookies to requests.
/// </summary>
public interface IValuesProviderService
{
    /// <summary>
    /// Edit window configuration.
    /// </summary>
    ValuesProviderSettings GetProviderSettings();

    // /// <summary>
    // /// Returns list of values will be injected to requests.
    // /// </summary>
    // [Obsolete]
    // Task<IReadOnlyCollection<ExtensionValueModel>> GetInjectionAsync(Guid settingsId, IReadOnlyCollection<ExtensionValueModel> userValues, bool forceReload = false);

    /// <summary>
    /// When user select to reload/rebuild values.
    /// </summary>
    Task<ReloadValuesResults> ReloadValuesAsync(IReadOnlyCollection<ValueModel> inputValues, ValuesReloadMode mode);
}
