using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[SuppressMessage("ReSharper", "AllUnderscoreLocalParameterName")]
[SuppressMessage("ReSharper", "CheckNamespace")]
partial class Build
{
    Target Solution_Clean => _ => _
        .Executes(() =>
        {
            if (Directory.Exists(OutputDirectory)) Directory.Delete(OutputDirectory, true);
            DotNetClean(x => x.SetProject(Solution));

            // delete all 'bin' directories
            foreach (var project in Solution.AllProjects.Where(x => !x.Name.Equals("Builder.Shared", StringComparison.Ordinal)))
            {
                var binDir = project.Directory / "bin";
                if (Directory.Exists(binDir)) Directory.Delete(binDir, true);
            }
        });

    Target Solution_Restore => _ => _
        .After(Solution_Clean)
        .Executes(() =>
        {
            DotNetRestore(x => x.SetProjectFile(Solution));
        });

    Target Solution_Build => _ => _
        .DependsOn(Solution_Restore)
        .Executes(() =>
        {
            // build all projects except 'Builder.Shared'
            foreach (var project in Solution.AllProjects.Where(x => !x.Name.Equals("Builder.Shared", StringComparison.Ordinal)))
                DotNetBuild(x => x.SetProjectFile(project));
        });
}
