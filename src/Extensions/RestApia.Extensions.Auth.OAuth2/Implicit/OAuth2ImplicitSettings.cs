namespace RestApia.Extensions.Auth.OAuth2.Implicit;

public record OAuth2ImplicitSettings
{
    public required string AuthUrl { get; init; }
    public required string RedirectUrl { get; init; }
    public required string ClientId { get; init; }
    public string Scopes { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
}
