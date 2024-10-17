using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestApia.Shared.Common;
using RestApia.Shared.Common.Models;
using RestApia.Shared.Extensions.ImportService;
namespace RestApia.Extensions.Import.Postman.Logic;

internal static class ImportCollection
{
    public static ImportedFileModel Import(JObject data, string path)
    {
        if (data["item"] is not JArray items)
            return new ImportedFileModel { Name = "Collection empty", Path = path, Icon = ImportFileIcon.Error };

        var name = data["info"]?["name"]?.Value<string>() ?? Path.GetFileNameWithoutExtension(path).ToCapitalCase();
        var requests = items.SelectMany(ReadCollectionItem);
        var workspaceValues = Tools.ParseValuesContent(data["variable"] as JArray ?? []);

        return new ImportedFileModel
        {
            Name = name,
            Path = path,
            Icon = ImportFileIcon.Collection,
            CollectionItems = [..requests],
            WorkspaceVariables = [new ValuesContentModel { Name = "Global values", Content = workspaceValues }],
            WorkspaceAuth = new[] { ReadAuth(data["auth"]) }.OfType<AuthContentModel>().ToList(),
        };
    }

    internal static AuthContentModel? ParseAuthUnsupported(JToken auth)
    {
        var type = auth["type"]?.Value<string>();
        if (type.IsEmpty()) return null;

        var settings = auth[type] as JArray;
        if (settings == null) return null;

        return new AuthContentModel
        {
            Name = $"{type} - Unsupported",
            SettingsClassFullName = string.Empty,
            Content = new[]
            {
                $"// Authorization type: '{type}'",
                "// Import of this authorization type is not supported yet (feel free to ping us with feature request)",
                "// https://github.com/RestApia/RestApia.Community/issues/new/choose",
                string.Empty,
                "// Postman settings source:",
                settings
                    .ToString(Formatting.Indented)
                    .SplitRegex("\r\n|\r|\n")
                    .Select(x => $"// {x}")
                    .JoinString("\r\n"),
            }.JoinString("\r\n", includeSpaces: true),
        };
    }

    private static IEnumerable<ImportedCollectionItem> ReadCollectionItem(JToken item)
    {
        if (item is null)
            yield break;

        var request = item["request"];
        if (request != null)
        {
            // request item
            yield return new ImportedCollectionItem
            {
                ItemType = ImportedCollectionItemType.Request,
                Name = item["name"]?.Value<string>() ?? "Unnamed Request",
                Content = BuildRequestContent(request),

                Environments = [],
                Values = [],
                Authorizations = new[] { ReadAuth(request["auth"]) }.OfType<AuthContentModel>().ToList(),

                Children = [],
            };
        }
        else
        {
            // folder item
            var children = item["item"] is JArray { Count: > 0 } items
                ? items.SelectMany(ReadCollectionItem).ToList()
                : [];

            yield return new ImportedCollectionItem
            {
                ItemType = ImportedCollectionItemType.Folder,
                Name = item["name"]?.Value<string>() ?? "Unnamed Folder",

                Environments = [],
                Values = [],
                Authorizations = (item["auth"] as JArray ?? []).Select(ReadAuth).OfType<AuthContentModel>().ToList(),

                Children = children,
            };
        }
    }

    private static string BuildRequestContent(JToken request)
    {
        var result = new List<string>();

        // variables
        var variables = request["variable"] as JArray ?? [];
        if (variables.Count > 0)
        {
            var variablesContent = Tools.ParseValuesContent(variables);
            if (variablesContent.IsNotEmpty())
                result.AddRange(["// variables", variablesContent, string.Empty]);
        }

        // endpoint
        var method = request["method"]?.Value<string>() ?? "GET";
        var urlDetails = ParseUrl(request["url"]);
        if (urlDetails.Values.IsNotEmpty()) result.AddRange([string.Empty, urlDetails.Values, string.Empty]);
        result.AddRange(["// endpoint", $"{method.ToUpper()} {urlDetails.Endpoint}", string.Empty]);

        // headers
        var headers = request["header"] as JArray ?? [];
        if (headers.Count > 0)
        {
            var headersContent = Tools.ParseValuesContent(headers, namePrefix: "@", ignoreKeys: ["content-type"]);

            if (headersContent.IsNotEmpty())
                result.AddRange(["// headers", headersContent, string.Empty]);
        }

        // body
        var body = request["body"];
        if (body != null)
            result.Add(ParseBody(body));

        return result.JoinString("\n", includeSpaces: true).Trim();
    }

    private static (string Endpoint, string Values) ParseUrl(JToken? data)
    {
        var urlEndpoint = data?["raw"]?.Value<string>() ?? string.Empty;
        var urlValues = Tools.ParseValues(data?["variable"] as JArray ?? []);

        foreach (var value in urlValues)
            urlEndpoint = urlEndpoint.Replace(":" + value.Key, "{{" + value.Key + "}}");

        return (urlEndpoint, Tools.SerializeValues(urlValues));
    }

    private static string ParseBody(JToken body)
    {
        var mode = body["mode"]?.Value<string>() ?? "raw";
        var (type, content) = mode.ToLower() switch
        {
            "raw" when body["options"]?["raw"]?["language"]?.Value<string>()?.Equals("json", StringComparison.OrdinalIgnoreCase) == true => (string.Empty, body["raw"]?.Value<string>() ?? string.Empty),
            "raw" => ("raw text/plain", body["raw"]?.Value<string>() ?? string.Empty),
            "urlencoded" => ("form", Tools.ParseValuesContent(body["urlencoded"] as JArray ?? [])),
            "formdata" => ("multipart", Tools.ParseValuesContent(body["formdata"] as JArray ?? [])),
            _ => (string.Empty, $"// Import of content type '{mode}' is not supported yet (feel free to ping us with feature request)\n// https://github.com/RestApia/RestApia.Documentation/issues/new/choose"),
        };

        if (content.IsEmpty()) return string.Empty;

        var result = type.IsEmpty() ? string.Empty : $"#{type.ToLowerInvariant()}";
        result += "\n" + content;

        return result.Trim();
    }

    private static AuthContentModel? ReadAuth(JToken? data) => data?["type"]?.Value<string?>()?.ToLowerInvariant() switch
    {
        "oauth2" => ImportAuthOAuth2.Import(data),
        "basic" => ImportAuthBasic.Import(data),
        "" or null => null,
        _ => ParseAuthUnsupported(data),
    };
}
