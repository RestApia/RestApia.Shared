using RestApia.Shared.Common.Enums;
namespace RestApia.Shared.Common.Services;

public static class PackageReader
{
    public static Dictionary<PackageRequirement, string> RequirementsRead(string source)
    {
        var result = source
            .Split(';')
            .Select(x =>
            {
                var splitIndex = x.IndexOfAny([':', '=']);
                return new
                {
                    Key = splitIndex == -1 ? x : x[..splitIndex].Trim(),
                    Value = splitIndex == -1 ? string.Empty : x[(splitIndex + 1)..].Trim(),
                };
            })
            .Select(x => new
            {
                Requirement = Enum.TryParse<PackageRequirement>(x.Key, true, out var requirement)
                    ? requirement
                    : PackageRequirement.MinAppVersion,
                x.Value,
            })
            .ToDictionary(x => x.Requirement, x => x.Value);

        return result;
    }
}
