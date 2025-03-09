using System.Diagnostics.CodeAnalysis;
using System.Web;
using RestApia.Shared.Common;
using RestApia.Shared.Common.Enums;
using RestApia.Shared.Common.Interfaces;
using RestApia.Shared.Common.Models;
using RestApia.Shared.Common.Services;
using RestApia.Shared.Extensions.ValuesProviderService.Enums;
using RestApia.Shared.Extensions.ValuesProviderService.Interfaces;
using RestApia.Shared.Extensions.ValuesProviderService.Models;
namespace RestApia.Extensions.Auth.OAuth2.Implicit;

public class OAuth2ImplicitService: IAuthValuesProvider
{
    private static readonly ValuesProviderSettings Settings = new ()
    {
        Title = "OAuth2: Implicit",
        CanBeReloaded = true,
        ReservedValues =
        [
            new ()
            {
                Name = nameof(OAuth2ImplicitSettings.AuthUrl),
                Description = "Authorization URL",
                IsRequired = true,
                Placeholder = "https://example.com/oauth2/authorize",
            },

            new ()
            {
                Name = nameof(OAuth2ImplicitSettings.RedirectUrl),
                Description = "Redirect URL",
                IsRequired = true,
                Placeholder = "https://example.com/oauth2/callback",
            },

            new ()
            {
                Name = nameof(OAuth2ImplicitSettings.ClientId),
                Description = "Client ID",
                IsRequired = true,
                Placeholder = Guid.Empty.ToString(),
            },

            new ()
            {
                Name = nameof(OAuth2ImplicitSettings.Scopes),
                Description = "Scopes",
                Placeholder = "email; profile",
            },

            new ()
            {
                Name = nameof(OAuth2ImplicitSettings.Audience),
                Description = "Audience",
                Placeholder = "https://example.com/api",
            },
        ],
    };

    private readonly ILogger _logger;
    private readonly IExtensionDialogs _dialogs;

    public OAuth2ImplicitService(ILogger logger, IExtensionDialogs dialogs)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static")]
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
        if (!VariablesConverter.TryDeserialize<OAuth2ImplicitSettings>(inputValues, out var settings))
            return ReloadValuesResults.FailedWithMessage("Authorization settings are invalid");

        // build start URL
        var scopes = settings.Scopes.SplitRegex(";, ");

        var startUrl = OAuth2Helper.AddQueryParams(settings.AuthUrl, [
            new ("response_type", "token"),
            new ("client_id", settings.ClientId),
            new ("redirect_uri", settings.RedirectUrl),
            new ("scope", string.Join(' ', scopes)),
            new ("audience", settings.Audience),
            new ("nonce", OAuth2Helper.Rnd.NextInt64().ToString()),
        ]);

        if (string.IsNullOrWhiteSpace(startUrl))
            return ReloadValuesResults.FailedWithMessage("Cannot open Authorization page, URL build from parameters is empty");

        // build stop URL
        var stopUrl = settings.RedirectUrl;
        if (string.IsNullOrWhiteSpace(stopUrl))
            return ReloadValuesResults.FailedWithMessage("Redirect URL cannot be empty");

        // var result = await AuthWindow.OpenDialogAsync(startUrl, stopUrl, context.AuthDna.Name, context.ParentWindow, context.Services);
        var response = await _dialogs.OpenAuthBrowserAsync(startUrl, stopUrl, "OAuth2 Implicit");
        if (response == null)
        {
            _logger.Debug("Auth canceled by user");
            return ReloadValuesResults.Canceled;
        }

        // validate response and extract access token
        var errorMessage = TryGetTokenOrError(response, out var tokenString);
        if (!string.IsNullOrWhiteSpace(errorMessage))
            return ReloadValuesResults.FailedWithMessage(errorMessage);

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
                IsSecret = true,
            },
        ];
        return new ()
        {
            Values = [..values, ..OAuth2Helper.GetTokenValues(values, _logger)],
            ExpiredAt = expiresAt,
            Status = ValueReloadResultType.Success,
        };
    }

    private static string TryGetTokenOrError(BrowserDialogResult result, out string? token)
    {
        // read response details
        token = null;
        var hashParts = result.Url.Split('#');

        if (hashParts.Length < 2)
            return "Empty hash part found in response for OAuth2 implicit auth";

        var queryString = HttpUtility.ParseQueryString(hashParts[1]);
        if (queryString == null)
            return "Empty hash part found in response for OAuth2 implicit auth";

        // check if error returned
        var errorCode = queryString["error"];
        if (!string.IsNullOrWhiteSpace(errorCode))
        {
            var errorDescription = queryString["error_description"];
            return $"Error response received: {errorDescription} ({errorCode})";
        }

        // extract token
        token = queryString["access_token"]!;
        if (string.IsNullOrWhiteSpace(token))
            return "Empty token received in response";

        return string.Empty;
    }
}
