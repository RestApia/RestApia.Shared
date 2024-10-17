using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[SuppressMessage("ReSharper", "AllUnderscoreLocalParameterName")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "CheckNamespace")]
partial class Build
{
    [Parameter("Extension library name")]
    string ExtensionName = EnvironmentInfo.GetVariable<string>("EXTENSION_NAME") ?? string.Empty;

    [Parameter("Extension library version")]
    string ExtensionLibVersion = EnvironmentInfo.GetVariable<string>("EXTENSION_VERSION") ?? string.Empty;

    AbsolutePath ExtensionDirectory => OutputDirectory / ExtensionName;

    Target Extension_FindNextVersion => _ => _
        .Requires(() => !string.IsNullOrWhiteSpace(ExtensionName))
        .OnlyWhenDynamic(() => string.IsNullOrWhiteSpace(ExtensionLibVersion))
        .Executes(async () =>
        {
            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            var resources = await repository.GetResourceAsync<FindPackageByIdResource>();
            var metadata = await resources.GetAllVersionsAsync(ExtensionName, new SourceCacheContext(), NullLogger.Instance, CancellationToken.None);
            var currentVersion = metadata
                .OrderByDescending(x => x)
                .FirstOrDefault() ?? new NuGetVersion("0.1.0");

            ExtensionLibVersion = currentVersion.Version.Major + "." + currentVersion.Version.Minor + "." + (currentVersion.Version.Build + 1);
            Log.Information("Extension library version: {SharedLibVersion}", ExtensionLibVersion);
        });

    Target Extension_Build => _ => _
        .Requires(() => !string.IsNullOrWhiteSpace(ExtensionName))
        .DependsOn(Solution_Restore, Extension_FindNextVersion)
        .Executes(() =>
        {
            var project = Solution
                .SolutionFolders
                .First(x => x.Name.Equals("Extensions", StringComparison.Ordinal))
                .Projects
                .FirstOrDefault(x => x.Name.Equals(ExtensionName, StringComparison.Ordinal));

            if (project == null) throw new FileNotFoundException($"Extension library project '{ExtensionName}' wasn't found.");

            DotNetBuild(x => x
                .SetProjectFile(project)
                .SetConfiguration(Configuration)
                .SetVersion(ExtensionLibVersion)
                .SetOutputDirectory(ExtensionDirectory));
        });

    Target Extension_Push => _ => _
        .Requires(() => !string.IsNullOrWhiteSpace(ExtensionName))
        .DependsOn(Extension_Build)
        .Executes(() =>
        {
            var path = Directory.GetFiles(ExtensionDirectory, $"{ExtensionName}.*.nupkg", SearchOption.AllDirectories).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(path)) throw new FileNotFoundException("Extension library wasn't found.");

            DotNetNuGetPush(x => x
                .SetTargetPath(path)
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetApiKey(PushApiKey));
        });
}
