using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Solution_Clean);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution]
    readonly Solution Solution;

    [CanBeNull]
    JObject _settings;

    JObject Settings => _settings ??= !File.Exists(RootDirectory / "settings.local.json5")
        ? new JObject()
        : JObject.Parse(File.ReadAllText(RootDirectory / "settings.local.json5"));

    // paths
    AbsolutePath OutputDirectory => RootDirectory / ".local" / "builds";
}
