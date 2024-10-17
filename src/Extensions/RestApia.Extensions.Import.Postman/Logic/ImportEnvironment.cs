using Newtonsoft.Json.Linq;
using RestApia.Shared.Common;
using RestApia.Shared.Common.Models;
using RestApia.Shared.Extensions.ImportService;
namespace RestApia.Extensions.Import.Postman.Logic;

internal static class ImportEnvironment
{
    public static ImportedFileModel Import(JObject data, string path)
    {
        var values = data["values"] as JArray;
        if (values == null)
            return new ImportedFileModel { Name = "Environment empty", Path = path, Icon = ImportFileIcon.Error };

        var name = data["name"]?.Value<string>() ?? Path.GetFileNameWithoutExtension(path).ToCapitalCase();
        var valuesContent = Tools.ParseValuesContent(values);

        return new ImportedFileModel
        {
            Name = name,
            Path = path,
            Icon = ImportFileIcon.Environment,
            WorkspaceEnvironments =
            [
                new ValuesContentModel
                {
                    Name = name,
                    Content = valuesContent,
                },
            ],
        };
    }
}
