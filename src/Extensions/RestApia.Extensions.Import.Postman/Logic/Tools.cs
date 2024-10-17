using Newtonsoft.Json.Linq;
using RestApia.Shared.Common;
namespace RestApia.Extensions.Import.Postman.Logic;

internal static class Tools
{
    public static IReadOnlyCollection<KeyValuePair<string, string>> ParseValues(JArray data, IReadOnlyCollection<string>? ignoreKeys = null) => data
        .Where(value => value["enabled"]?.Value<bool>() != false)
        .Select(value => new
        {
            Key = value["key"]?.Value<string>(),
            Value = value["value"]?.Value<string>(),
        })
        .Where(value => value.Key.IsNotEmpty() && ignoreKeys?.Contains(value.Key, StringComparer.OrdinalIgnoreCase) != true)
        .Select(x => new KeyValuePair<string, string>(x.Key!, x.Value ?? string.Empty))
        .ToList();

    public static string ParseValuesContent(JArray data, string namePrefix = "", IReadOnlyCollection<string>? ignoreKeys = null) =>
        SerializeValues(ParseValues(data, ignoreKeys), namePrefix);

    public static string SerializeValues(IReadOnlyCollection<KeyValuePair<string, string>> values, string namePrefix = "") => values
        .Select(value => $"{namePrefix}{value.Key}: {value.Value ?? string.Empty}")
        .JoinString("\r\n");
}
