using RestApia.Shared.Common.Attributes;
namespace RestApia.Extensions.Auth.OAuth2.AuthCode;

public record OAuth2AuthCodeSettings
{
    [ValuesContentItem("AuthUrl", "Authorization URL", IsRequired = true)]
    public required string AuthUrl { get; init; }

    [ValuesContentItem("TokenUrl", "Access token URL", IsRequired = true)]
    public required string AccessTokenUrl { get; init; }

    [ValuesContentItem("RedirectUrl", "Redirect URL", IsRequired = true)]
    public required string RedirectUrl { get; init; }

    [ValuesContentItem("ClientId", "Client Id", IsRequired = true)]
    public required string ClientId { get; init; }

    [ValuesContentItem("ClientSecret", "Client secret")]
    public string ClientSecret { get; init; } = string.Empty;

    [ValuesContentItem("SendMethod", "Credentials send method. Can be 'Header' or 'Body'")]
    public required string CredentialsSendMethod { get; init; }

    [ValuesContentItem("Scopes", "List of scopes, separated by space, comma or semicolon")]
    public string Scopes { get; init; } = string.Empty;

    [ValuesContentItem("Audience", "Audience")]
    public string Audience { get; init; } = string.Empty;

    [ValuesContentItem("State", "State")]
    public string State { get; init; } = string.Empty;

    [ValuesContentItem("Resource", "Resource")]
    public string Resource { get; init; } = string.Empty;

    [ValuesContentItem("Origin", "Origin")]
    public string Origin { get; init; } = string.Empty;
}
