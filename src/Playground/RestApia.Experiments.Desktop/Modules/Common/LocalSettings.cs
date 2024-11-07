using Newtonsoft.Json.Linq;
namespace RestApia.Experiments.Desktop.Modules.Common;

public static class LocalSettings
{
    private const string SettingsFileName = "settings.local.json5";
    private static readonly JObject JsonInternal = ReadSettings();

    public static T Get<T>() => Get<T>(typeof(T).Name);
    public static T Get<T>(string key)
    {
        var json = JsonInternal[key];
        if (json == null) throw new KeyNotFoundException($"Key '{key}' not found in settings");

        var result = json.ToObject<T>();
        if (result == null) throw new InvalidCastException($"Cannot convert value for key '{key}' to type '{typeof(T).Name}'");

        return result;
    }

    private static JObject ReadSettings()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), SettingsFileName);

        if (!File.Exists(filePath))
            return new JObject();

        var json = File.ReadAllText(filePath);
        return JObject.Parse(json);
    }
}
