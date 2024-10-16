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

[SuppressMessage("ReSharper", "CheckNamespace")]
[SuppressMessage("ReSharper", "AllUnderscoreLocalParameterName")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
partial class Build
{
    AbsolutePath SharedDirectory => OutputDirectory / "RestApia.Shared";

    [Parameter("Shared library version")]
    string SharedLibVersion = EnvironmentInfo.GetVariable<string>("SHARED_LIB_VERSION") ?? string.Empty;

    [Parameter("NuGet push API key")]
    string PushApiKey => EnvironmentInfo.GetVariable<string>("NUGET_API") ?? Settings.Value<string>("nuget_push_api_key");

    Target Shared_FindNextVersion => _ => _
        .OnlyWhenDynamic(() => string.IsNullOrWhiteSpace(SharedLibVersion))
        .Executes(async () =>
        {
            // find nuget with name "RestApia.Shared" and get next possible version
            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            var resources = await repository.GetResourceAsync<FindPackageByIdResource>();
            var metadata = await resources.GetAllVersionsAsync("RestApia.Shared", new SourceCacheContext(), NullLogger.Instance, CancellationToken.None);
            var currentVersion = metadata
                .OrderByDescending(x => x)
                .FirstOrDefault() ?? new NuGetVersion("0.1.0");

            SharedLibVersion = currentVersion.Version.Major + "." + currentVersion.Version.Minor + "." + (currentVersion.Version.Build + 1);
            Log.Information("Shared library version: {SharedLibVersion}", SharedLibVersion);
        });

    Target Shared_Build => _ => _
        .DependsOn(Solution_Restore, Shared_FindNextVersion)
        .Executes(() =>
        {
            DotNetBuild(x => x
                .SetProjectFile(Solution.Projects.First(x => x.Name.Equals("RestApia.Shared", StringComparison.Ordinal)))
                .SetConfiguration(Configuration)
                .SetVersion(SharedLibVersion)
                .SetOutputDirectory(SharedDirectory));
        });

    Target Shared_Push => _ => _
        .DependsOn(Shared_Build)
        .Executes(() =>
        {
            var path = Directory.GetFiles(SharedDirectory, "*.nupkg", SearchOption.AllDirectories).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(path)) throw new FileNotFoundException("Shared library wasn't found.");

            DotNetNuGetPush(x => x
                .SetTargetPath(path)
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetApiKey(PushApiKey));
        });
}
