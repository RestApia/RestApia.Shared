using RestApia.Shared.Common.Models;
using RestApia.Shared.Extensions.ValuesProviderService.Enums;
using RestApia.Shared.Extensions.ValuesProviderService.Interfaces;
using RestApia.Shared.Extensions.ValuesProviderService.Models;
namespace RestApia.Extensions.ValuesProvider.CollectionValuesProvider;

public class UserValuesProvider: IUserValuesProvider
{
    // no reserved values or custom controls
    public ValuesProviderSettings GetProviderSettings() => new ()
    {
        Title = "User Values",
        CanBeReloaded = false,
        DisableCachingResults = true,
        HelpPageUrl = "https://github.com/RestApia/RestApia.Shared/tree/main/src/Extensions/RestApia.Extensions.ValuesProvider.CollectionValuesProvider",
    };

    public Task<ReloadValuesResults> ReloadValuesAsync(IReadOnlyCollection<ValueModel> inputValues, ValuesReloadMode mode) =>
        Task.FromResult(new ReloadValuesResults { Values = inputValues, Status = ValueReloadResultType.Success });
}
