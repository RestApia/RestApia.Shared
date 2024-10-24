using Newtonsoft.Json.Linq;
using RestApia.Shared.Common.Models;
namespace RestApia.Extensions.Import.Postman.Logic;

internal static class ImportAuthOAuth2
{
    public static AuthContentModel? Import(JToken data)
    {
        var values = Tools.ParseValues(data["oauth2"] as JArray ?? [])
            .ToDictionary(x => x.Key, x => x.Value);

        var grandType = values.GetValueOrDefault("grant_type", string.Empty);
        return grandType switch
        {
            "authorization_code" => ParseAuthOAuth2AuthorizationCode(values),
            _ => ParseAuthOAuth2UnknownGrandType(data),
        };
    }

    private static AuthContentModel ParseAuthOAuth2AuthorizationCode(Dictionary<string, string> values)
    {
        // OAuth2AuthCodeSettings object
        var settings = new Dictionary<string, string>
        {
            { "AuthUrl", values.GetValueOrDefault("authUrl", string.Empty) },
            { "AccessTokenUrl", values.GetValueOrDefault("accessTokenUrl", string.Empty) },
            { "RedirectUrl", values.GetValueOrDefault("redirect_uri", string.Empty) },
            { "ClientId", values.GetValueOrDefault("clientId", string.Empty) },
            { "ClientSecret", values.GetValueOrDefault("clientSecret", string.Empty) },
            { "Scopes", values.GetValueOrDefault("scope", string.Empty) },
            { "CredentialsSendMethod", values.GetValueOrDefault("client_authentication", string.Empty).Equals("body", StringComparison.OrdinalIgnoreCase) ? "Body" : "Header" },
        };

        return new AuthContentModel
        {
            Name = "OAuth2 - Authorization Code",
            ProviderClassFullName = "RestApia.Extensions.Auth.OAuth2.AuthCode.OAuth2AuthCodeService",
            Content = Tools.SerializeValues(settings),
        };
    }

    private static AuthContentModel? ParseAuthOAuth2UnknownGrandType(JToken data)
    {
        var result = ImportCollection.ParseAuthUnsupported(data);
        if (result == null) return null;

        return result with
        {
            Name = "OAuth2 - Unsupported grant type",
            Content = $"// Unsupported OAuth2 grant type: {data["grant_type"]}\r\n" + result.Content,
        };
    }
}
