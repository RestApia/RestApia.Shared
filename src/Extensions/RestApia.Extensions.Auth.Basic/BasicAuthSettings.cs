using RestApia.Shared.Common.Attributes;
namespace RestApia.Extensions.Auth.Basic;

public record BasicAuthSettings
{
    [ValuesContentItem("Name", "User login name", IsRequired = true)]
    public string UserName { get; init; } = string.Empty;

    [ValuesContentItem("Password", "User password", IsRequired = true)]
    public string Password { get; init; } = string.Empty;
}
