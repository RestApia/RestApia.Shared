namespace RestApia.Shared.Common.Models;

public record BoolResult
{
    public bool Value => this is TrueResult;

    public static implicit operator bool(BoolResult result) => result is TrueResult;
    public static implicit operator BoolResult(bool result) => result ? True : False;

    public static BoolResult True { get; } = new TrueResult();
    public static BoolResult False { get; } = new FalseResult();
    public static BoolResult FalseError(string error) => new FalseResult(error);
}

public record TrueResult : BoolResult;
public record FalseResult(string? Message = null) : BoolResult;
