namespace RestApia.Extensions.Auth.Basic;

public record BasicAuthSettings
{
    public string Name { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
