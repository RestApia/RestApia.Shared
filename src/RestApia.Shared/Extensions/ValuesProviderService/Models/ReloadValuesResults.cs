using RestApia.Shared.Common.Models;
using RestApia.Shared.Extensions.ValuesProviderService.Enums;
namespace RestApia.Shared.Extensions.ValuesProviderService.Models;

public record ReloadValuesResults
{
    public required IReadOnlyCollection<ValueModel> Values { get; init; }
    public required ValueReloadResultType Status { get; init; } = ValueReloadResultType.Success;
    public string ErrorMessage { get; init; } = string.Empty;
    public DateTime? ExpiredAt { get; init; }

    public static ReloadValuesResults Canceled { get; } = new () { Values = [], Status = ValueReloadResultType.Canceled };
    public static ReloadValuesResults Failed { get; } = new () { Values = [], Status = ValueReloadResultType.Failed };
    public static ReloadValuesResults FailedWithMessage(string errorMessage) => new ()
    {
        Values = [],
        Status = ValueReloadResultType.Failed,
        ErrorMessage = errorMessage,
    };
}
