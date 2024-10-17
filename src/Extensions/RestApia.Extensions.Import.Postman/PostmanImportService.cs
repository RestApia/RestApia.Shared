using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestApia.Extensions.Import.Postman.Abstract;
using RestApia.Extensions.Import.Postman.Logic;
using RestApia.Shared.Common;
using RestApia.Shared.Extensions.ImportService;

namespace RestApia.Extensions.Import.Postman;

/// <summary>
/// Postman import service.
/// </summary>
public class PostmanImportService : IImportService
{
    /// <inheritdoc />
    public ImportedFileModel Import(string path)
    {
        if (!ReadFile(path, out var content))
            return new ImportedFileModel { Name = "File not found", Path = path, Icon = ImportFileIcon.Error };

        if (!ReadJData(content, out var jData))
            return new ImportedFileModel { Name = "File is empty", Path = path, Icon = ImportFileIcon.Error };

        return jData.Type switch
        {
            PostmanJsonType.Collection => ImportCollection.Import(jData.Data, path),
            PostmanJsonType.Environment => ImportEnvironment.Import(jData.Data, path),
            _ => new ImportedFileModel { Name = "Unknown file type", Path = path, Icon = ImportFileIcon.Error },
        };
    }

    private static bool ReadJData(string content, [NotNullWhen(true)] out JDataModel? result)
    {
        result = null;
        var obj = JsonConvert.DeserializeObject<JObject>(content);
        var type = PostmanJsonType.Unknown;

        // if not deserialize
        if (obj == null) return false;

        if (!string.IsNullOrWhiteSpace(obj["info"]?["name"]?.Value<string>())) type = PostmanJsonType.Collection;
        if (!string.IsNullOrWhiteSpace(obj["name"]?.Value<string>()) && obj["values"] is JArray) type = PostmanJsonType.Environment;

        result = new (obj, type);
        return true;
    }

    private static bool ReadFile(string path, [NotNullWhen(true)] out string? result)
    {
        result = null;
        if (!File.Exists(path)) return false;

        result = File.ReadAllText(path);
        if (result.IsEmpty()) return false;

        return true;
    }
}
