using System.Diagnostics.CodeAnalysis;
using System.Text;
using RestApia.Shared.Common;
using RestApia.Shared.Common.Enums;
using RestApia.Shared.Common.Models;
using RestApia.Shared.Common.Services;
using RestApia.Shared.Extensions.ValuesProviderService.Enums;
using RestApia.Shared.Extensions.ValuesProviderService.Interfaces;
using RestApia.Shared.Extensions.ValuesProviderService.Models;
namespace RestApia.Extensions.Auth.Basic;

public class BasicAuthService: IAuthValuesProvider
{
    private static readonly ValuesProviderSettings Settings = new ()
    {
        Title = "Basic Authorization",
        CanBeReloaded = true,
        DisableCachingResults = true,
        ReservedValues =
        [
            new () { Name = nameof(BasicAuthSettings.Name), Description = "User name", IsRequired = true, Placeholder = "User" },
            new () { Name = nameof(BasicAuthSettings.Password), Description = "User Password", Placeholder = "secret" },
        ],
    };

    public ValuesProviderSettings GetProviderSettings() => Settings;

    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public Task<ReloadValuesResults> ReloadValuesAsync(IReadOnlyCollection<ValueModel> inputValues, ValuesReloadMode mode) =>
        Task.FromResult(ReloadValues(inputValues));

    private static ReloadValuesResults ReloadValues(IReadOnlyCollection<ValueModel> inputValues)
    {
        // all templated values must be replaced
        var hasUnresolvedValues = inputValues.All(x => x.Value.Parts.All(y => !y.IsTemplatedVariable));
        if (!hasUnresolvedValues) return ReloadValuesResults.Failed;

        if (!VariablesConverter.TryDeserialize<BasicAuthSettings>(inputValues, out var settings))
            return ReloadValuesResults.Failed;

        // validate values
        var settingsErrors = new List<string>();
        if (settings.Name.IsEmpty())
            settingsErrors.Add($"{nameof(settings.Name)} value cannot be empty");

        if (settingsErrors.Count > 0)
            return ReloadValuesResults.FailedWithMessage(settingsErrors.JoinString("\n"));

        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{settings.Name}:{settings.Password}"));
        IReadOnlyCollection<ValueModel> result =
        [
            new () { Name = "Authorization", Type = ValueTypeEnum.Header, Value = $"Basic {token}" },
        ];

        return new ReloadValuesResults { Values = result, Status = ValueReloadResultType.Success };
    }
}
