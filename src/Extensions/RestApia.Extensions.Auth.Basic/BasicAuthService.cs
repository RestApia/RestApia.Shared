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
        Title = "Basic Authentication",
        DefaultName = "Basic Auth",
        Description = "Provide your login ID and password. RestApia will take these details and include them in the request header using the Basic Authentication standard.",
        CanBeReloaded = true,
        DisableCachingResults = true,
        ReservedValues =
        [
            new () { Name = nameof(BasicAuthSettings.Name), Description = "Login identifier or username", IsRequired = true, Placeholder = "user" },
            new () { Name = nameof(BasicAuthSettings.Password), Description = "Your secret password", Placeholder = "password" },
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
