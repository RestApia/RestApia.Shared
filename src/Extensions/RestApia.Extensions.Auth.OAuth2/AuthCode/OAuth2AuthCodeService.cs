using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestApia.Shared.Common;
using RestApia.Shared.Common.Enums;
using RestApia.Shared.Common.Interfaces;
using RestApia.Shared.Common.Models;
using RestApia.Shared.Common.Services;
using RestApia.Shared.Extensions.ValuesProviderService.Enums;
using RestApia.Shared.Extensions.ValuesProviderService.Interfaces;
using RestApia.Shared.Extensions.ValuesProviderService.Models;
namespace RestApia.Extensions.Auth.OAuth2.AuthCode;

public class OAuth2AuthCodeService: IAuthValuesProvider
{
    private static readonly HttpClient AccessTokenHttpClient = new ();

    private static readonly ValuesProviderSettings Settings = new ()
    {
        Title = "OAuth2: Auth Code",
        CanBeReloaded = true,

        ReservedValues =
        [
            new ()
            {
                Name = nameof(OAuth2AuthCodeSettings.AuthUrl),
                Description = "Authorization URL",
                IsRequired = true,
                Placeholder = "https://example.com/oauth2/authorize",
            },
            new ()
            {
                Name = nameof(OAuth2AuthCodeSettings.TokenUrl),
                Description = "Access Token URL",
                IsRequired = true,
                Placeholder = "https://example.com/oauth2/token",
            },

            new ()
            {
                Name = nameof(OAuth2AuthCodeSettings.RedirectUrl),
                Description = "Redirect URL",
                IsRequired = true,
                Placeholder = "https://example.com/oauth2/callback",
            },

            new ()
            {
                Name = nameof(OAuth2AuthCodeSettings.ClientId),
                Description = "Client ID",
                IsRequired = true,
                Placeholder = Guid.Empty.ToString(),
            },

            new ()
            {
                Name = nameof(OAuth2AuthCodeSettings.ClientSecret),
                Description = "Client Secret",
                Placeholder = "****************",
            },

            new ()
            {
                Name = nameof(OAuth2AuthCodeSettings.SendMethod),
                Description = "Credentials sending method",
                Placeholder = "header or body",
                ExpectedValues = [string.Empty, null, "header", "body"], // empty string is
            },

            new ()
            {
                Name = nameof(OAuth2AuthCodeSettings.Scopes),
                Description = "Scopes",
                Placeholder = "email; profile",
            },

            new ()
            {
                Name = nameof(OAuth2AuthCodeSettings.Audience),
                Description = "Audience",
                Placeholder = "https://example.com/api",
            },

            new ()
            {
                Name = nameof(OAuth2AuthCodeSettings.State),
                Description = "State",
            },

            new ()
            {
                Name = nameof(OAuth2AuthCodeSettings.Resource),
                Description = "Resource",
            },

            new ()
            {
                Name = nameof(OAuth2AuthCodeSettings.Origin),
                Description = "Origin",
                Placeholder = "https://example.com",
            },
        ],
    };

    private readonly ILogger _logger;
    private readonly IExtensionDialogs _dialogs;

    public OAuth2AuthCodeService(ILogger logger, IExtensionDialogs dialogs)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
    }

    public ValuesProviderSettings GetProviderSettings() => Settings;

    public async Task<ReloadValuesResults> ReloadValuesAsync(IReadOnlyCollection<ValueModel> inputValues, ValuesReloadMode mode)
    {
        // all templated values must be replaced
        var hasUnresolvedValues = mode == ValuesReloadMode.Interactive && inputValues.All(x => x.Value.Parts.All(y => !y.IsTemplatedVariable));
        if (!hasUnresolvedValues) return ReloadValuesResults.Failed;

        // validate settings
        if (!Settings.ValidateReserved(inputValues, out var errors))
        {
            var resultError = errors.Count == 1
                ? errors.ElementAt(0)
                : errors.Select(x => $"- {x}").JoinString("\r\n");
            return ReloadValuesResults.FailedWithMessage(resultError);
        }

        // try to deserialize settings
        if (!VariablesConverter.TryDeserialize<OAuth2AuthCodeSettings>(inputValues, out var settings))
            return ReloadValuesResults.FailedWithMessage("Authorization settings are invalid");

        var (codeResponse, codeError) = await RequestCodeOrDefaultAsync(settings);
        if (codeError.IsNotEmpty()) return ReloadValuesResults.FailedWithMessage(codeError);
        if (codeResponse == null) return ReloadValuesResults.Canceled;

        if (!TryExtractCodeOrError(codeResponse, out var codeString, out var extractError))
        {
            return extractError.IsNotEmpty()
                ? ReloadValuesResults.FailedWithMessage(extractError)
                : ReloadValuesResults.Canceled;
        }

        var (tokenString, tokenError) = await RequestTokenOrDefaultAsync(codeString, settings);
        if (tokenError.IsNotEmpty()) return ReloadValuesResults.FailedWithMessage(tokenError);
        if (tokenString == null) return ReloadValuesResults.Canceled;

        // read from token when it will be expired
        DateTime? expiresAt = null;
        if (!OAuth2Helper.TryParseToken(tokenString, _logger, out var token))
        {
            _logger.Warn("Cannot validate JWT token, expiration date will not be set");
        }
        else
        {
            expiresAt = token.ValidTo;
        }

        IReadOnlyCollection<ValueModel> values =
        [
            new ()
            {
                Name = "Authorization",
                Type = ValueTypeEnum.Header,
                Value = $"Bearer {tokenString}",
            },
        ];

        return new ()
        {
            Values = [..values, ..OAuth2Helper.GetTokenValues(values, _logger)],
            ExpiredAt = expiresAt,
            Status = ValueReloadResultType.Success,
        };
    }

    private async Task<(BrowserDialogResult? DialogResult, string? Error)> RequestCodeOrDefaultAsync(OAuth2AuthCodeSettings settings)
    {
        var scopes = settings.Scopes.SplitRegex(";, ");
        var queryParams = new List<KeyValuePair<string, string>>
        {
            new ("response_type", "code"),
            new ("client_id", settings.ClientId),
            new ("redirect_uri", settings.RedirectUrl),
            new ("scope", string.Join(' ', scopes)),
            new ("state", settings.State),
            new ("audience", settings.Audience),
            new ("resource", settings.Resource),
            new ("nonce", OAuth2Helper.Rnd.NextInt64().ToString()),
        };

        var startUrl = OAuth2Helper.AddQueryParams(settings.AuthUrl, queryParams);
        if (string.IsNullOrWhiteSpace(startUrl))
        {
            return (null, "Cannot open Authorization page, URL build from parameters is empty");
        }

        // build stop URL
        var stopUrl = settings.RedirectUrl;
        if (string.IsNullOrWhiteSpace(stopUrl))
        {
            return (null, "Redirect URL cannot be empty");
        }

        var result = await _dialogs.OpenAuthBrowserAsync(startUrl, stopUrl, "OAuth2 Auth Code");
        return (result, null);
    }

    private static async Task<(string? Token, string? Error)> RequestTokenOrDefaultAsync(string codeString, OAuth2AuthCodeSettings settings)
    {
        // prepare request
        var requestParams = new List<KeyValuePair<string, string>>
        {
            new ("grant_type", "authorization_code"),
            new ("code", codeString),
            new ("redirect_uri", settings.RedirectUrl),
            new ("state", settings.State),
            new ("audience", settings.Audience),
            new ("resource", settings.Resource),
        };

        var headers = new List<KeyValuePair<string, string>>
        {
            new ("Accept", "application/x-www-form-urlencoded, application/json"),
            new ("Origin", settings.Origin),
        };

        var sendMethod = settings.SendMethod.IsEmpty() ? "header" : settings.SendMethod.ToLower().Trim();
        switch (sendMethod)
        {
            case "body":
                requestParams.Add(new ("client_id", settings.ClientId));
                requestParams.Add(new ("client_secret", settings.ClientSecret));
                break;

            default:
                var credentialsString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{settings.ClientId}:{settings.ClientSecret}"));
                headers.Add(new ("Authorization", "Basic " + credentialsString));
                break;
        }

        // send request
        using var request = new HttpRequestMessage(HttpMethod.Post, settings.TokenUrl);
        request.Content = new FormUrlEncodedContent(requestParams.Where(x => !x.Value.IsEmpty()));

        foreach (var header in headers.Where(x => !x.Value.IsEmpty()))
            request.Headers.Add(header.Key, header.Value);

        var response = await AccessTokenHttpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return (null, $"Cannot retrieve access token. HTTP Status code: {response.ReasonPhrase} ({response.StatusCode})");
        }

        // read response
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return (null, "Error occurred during authorization. Empty response received.");
        }

        var extracted = response.Content.Headers.ContentType?.MediaType?.Trim().Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) == true
            ? TryExtractTokenFromUrlEncoded(responseContent, out var result, out var extractError)
            : TryExtractTokenFromJson(responseContent, out result, out extractError);

        if (!extracted)
        {
            return (null, extractError.IsNotEmpty() ? extractError : "Error occurred during authorization. Token not found in response.");
        }

        return (result, null);
    }

    private static bool TryExtractTokenFromUrlEncoded(string responseContent, [NotNullWhen(true)] out string? token, [NotNullWhen(false)] out string? error)
    {
        token = null;
        error = null;

        // expected content: access_token=gho_tUWZ&scope=&token_type=bearer
        var responseParams = OAuth2Helper.GetQueryParams(responseContent);
        token = responseParams["access_token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            error = "Error occurred during authorization. Access token not found.";
            return false;
        }

        return !token.IsEmpty();
    }

    private static bool TryExtractTokenFromJson(string responseContent, out string? token, [NotNullWhen(false)] out string? error)
    {
        token = null;
        error = null;

        var responseJson = JsonConvert.DeserializeObject<JObject>(responseContent);

        /* expected format
        {
          "token_type": "Bearer",
          "scope": "read:example write:example",
          "expires_in": 4597,
          "ext_expires_in": 4597,
          "access_token": "eyJ0eXA..."
        } */

        if (responseJson == null)
        {
            error = "Error occurred during authorization. Response content is empty.";
            return false;
        }

        if (!responseJson.TryGetValue("access_token", out var accessTokenProp))
        {
            error = "Error occurred during authorization. Access token not found.";
            return false;
        }

        token = accessTokenProp.Value<string?>();
        if (string.IsNullOrWhiteSpace(token))
        {
            error = "Error occurred during authorization. Access token is empty.";
            return false;
        }

        return true;
    }

    private static bool TryExtractCodeOrError(BrowserDialogResult response, [NotNullWhen(true)] out string? code, [NotNullWhen(false)] out string? error)
    {
        code = null;
        error = null;

        var url = response.Url;

        if (string.IsNullOrWhiteSpace(url))
        {
            error = "Empty URL received in OAuth2 AuthCode flow window";
            return false;
        }

        var queryString = OAuth2Helper.GetQueryParams(url);

        // check errors
        var errorCode = queryString["error"];
        if (!string.IsNullOrWhiteSpace(errorCode))
        {
            var errorDescription = queryString["error_description"];
            error = $"Error response received: {errorDescription} ({errorCode})";
            return false;
        }

        // find code
        code = queryString["code"];
        if (string.IsNullOrWhiteSpace(code))
        {
            error = "No code found in OAuth2 AuthCode flow window response";
            return false;
        }

        return !string.IsNullOrWhiteSpace(code);
    }
}
