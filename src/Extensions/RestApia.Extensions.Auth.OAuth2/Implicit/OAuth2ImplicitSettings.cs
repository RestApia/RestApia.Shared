using RestApia.Shared.Common.Attributes;
namespace RestApia.Extensions.Auth.OAuth2.Implicit;

public record OAuth2ImplicitSettings
{
    [ValuesContentItem("AuthUrl", "Authorization URL", IsRequired = true)]
    public required string AuthUrl { get; init; }

    [ValuesContentItem("RedirectUrl", "Redirect URL", IsRequired = true)]
    public required string RedirectUrl { get; init; }

    [ValuesContentItem("ClientId", "Client Id", IsRequired = true)]
    public required string ClientId { get; init; }

    [ValuesContentItem("Scopes", "List of scopes, separated by space, comma or semicolon")]
    public string Scopes { get; init; } = string.Empty;

    [ValuesContentItem("Audience", "Audience")]
    public string Audience { get; init; } = string.Empty;
}
