namespace RestApia.Shared.Common.Models;

public record MenuItem
{
    public required string Title { get; init; }
    public string IconCode { get; init; } = "e860";
}
