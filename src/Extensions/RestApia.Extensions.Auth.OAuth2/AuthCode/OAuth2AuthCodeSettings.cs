namespace RestApia.Extensions.Auth.OAuth2.AuthCode;

public record OAuth2AuthCodeSettings
{
    public required string AuthUrl { get; init; }
    public required string TokenUrl { get; init; }
    public required string RedirectUrl { get; init; }
    public required string ClientId { get; init; }
    public string ClientSecret { get; init; } = string.Empty;
    public string? SendMethod { get; init; }
    public string Scopes { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string Resource { get; init; } = string.Empty;
    public string Origin { get; init; } = string.Empty;
}
