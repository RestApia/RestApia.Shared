using RestApia.Shared.Common.Models;
namespace RestApia.Shared.Extensions.AuthService;

public interface IAuthService
{
    string DisplayName { get; }
    bool IsReloadFeatureAvailable { get; }
    bool IsShowPayloadFeatureAvailable { get; }

    Type SettingsType { get; }
    Task<IReadOnlyCollection<ValueModel>> GetValuesAsync(object settingsObj, Guid authId);
    Task<bool> ReloadAsync(object settings, Guid authId);
}
