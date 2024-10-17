using System.Text;
using RestApia.Shared.Common.Enums;
using RestApia.Shared.Common.Models;
using RestApia.Shared.Extensions.AuthService;
namespace RestApia.Extensions.Auth.Basic;

public class BasicAuthService : IAuthService
{
    public Type SettingsType => typeof(BasicAuthSettings);

    public string DisplayName => "Basic Authorization";
    public bool IsReloadFeatureAvailable => false;
    public bool IsShowPayloadFeatureAvailable => true;

    public Task<IReadOnlyCollection<ValueModel>> GetValuesAsync(object settingsObj, Guid authId) => Task.FromResult(GetValues(settingsObj));

    private static IReadOnlyCollection<ValueModel> GetValues(object rawSettings)
    {
        if (rawSettings is not BasicAuthSettings settings) return [];
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{settings.UserName}:{settings.Password}"));

        return
        [
            new () { Name = "Authorization", Type = ValuesContentItemTypeEnum.Header, Value = $"Basic {token}" },
        ];
    }

    public Task<bool> ReloadAsync(object settings, Guid authId) => throw new NotSupportedException();
}
