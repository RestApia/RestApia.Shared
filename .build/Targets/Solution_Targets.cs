using System.IO;
using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    Target Solution_Clean => _ => _
        .Executes(() =>
        {
            if (Directory.Exists(OutputDirectory)) Directory.Delete(OutputDirectory, true);
            DotNetClean(x => x.SetProject(Solution));
        });

    Target Solution_Restore => _ => _
        .After(Solution_Clean)
        .Executes(() =>
        {
            DotNetRestore(x => x.SetProjectFile(Solution));
        });
}
