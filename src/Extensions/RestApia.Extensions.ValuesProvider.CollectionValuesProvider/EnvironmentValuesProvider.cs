using RestApia.Shared.Common.Models;
using RestApia.Shared.Extensions.ValuesProviderService.Enums;
using RestApia.Shared.Extensions.ValuesProviderService.Interfaces;
using RestApia.Shared.Extensions.ValuesProviderService.Models;
namespace RestApia.Extensions.ValuesProvider.CollectionValuesProvider;

public class EnvironmentValuesProvider: IEnvironmentValuesProvider
{
    // no reserved values or custom controls
    public ValuesProviderSettings GetProviderSettings() => new ()
    {
        Title = "Environment Values",
        DefaultName = "DEV",
        Description = "Environments help you keep your API interactions organized by separating configurations for different stages or versions of the APIs you are using. You can create multiple environments, each with its own set of values, headers and cookies.",
        CanBeReloaded = false,
        DisableCachingResults = true,
        HelpPageUrl = "https://github.com/RestApia/RestApia.Shared/tree/main/src/Extensions/RestApia.Extensions.ValuesProvider.CollectionValuesProvider",
    };

    public Task<ReloadValuesResults> ReloadValuesAsync(IReadOnlyCollection<ValueModel> inputValues, ValuesReloadMode mode) =>
        Task.FromResult(new ReloadValuesResults { Values = inputValues, Status = ValueReloadResultType.Success });
}
