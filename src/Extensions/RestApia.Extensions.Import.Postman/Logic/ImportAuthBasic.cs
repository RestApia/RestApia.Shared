using Newtonsoft.Json.Linq;
using RestApia.Shared.Common.Models;
namespace RestApia.Extensions.Import.Postman.Logic;

public static class ImportAuthBasic
{
    public static AuthContentModel Import(JToken data)
    {
        var values = Tools.ParseValues(data["basic"] as JArray ?? [])
            .ToDictionary(x => x.Key, x => x.Value);

        var settings = new Dictionary<string, string>
        {
            { "UserName", values.GetValueOrDefault("username", string.Empty) },
            { "Password", values.GetValueOrDefault("password", string.Empty) },
        };

        return new ()
        {
            Name = "Basic Authorization",
            Content = Tools.SerializeValues(settings),
            ProviderClassFullName = "RestApia.Extensions.Auth.Basic.BasicAuthService",
        };
    }
}
