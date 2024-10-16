using System.Diagnostics.CodeAnalysis;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Serilog;

// ReSharper disable UnusedMember.Local
// ReSharper disable AllUnderscoreLocalParameterName

[GitHubActions(
    "cd-publish-shared",
    GitHubActionsImage.WindowsLatest,
    InvokedTargets = [
        nameof(Shared_Push),
    ],
    ImportSecrets = ["NUGET_PUSH_SHARED"],
    OnWorkflowDispatchOptionalInputs = ["SharedLibVersion"]
)]

[GitHubActions(
    "cd-publish-extension",
    GitHubActionsImage.WindowsLatest,
    InvokedTargets = [
        nameof(Extension_Push),
    ],
    ImportSecrets = ["NUGET_PUSH_SHARED"],
    OnWorkflowDispatchRequiredInputs = ["ExtensionName"],
    OnWorkflowDispatchOptionalInputs = ["ExtensionLibVersion"]
)]

// Continuous Integration: Validate pull requests
[GitHubActions(
    "pr-validation",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = [
        nameof(Solution_Build),
    ],
    OnPullRequestBranches = ["master"]
)]
[SuppressMessage("ReSharper", "CheckNamespace")]

// CI/CD targets
partial class Build
{
    Target UpdateYaml => _ => _.Executes(() => Log.Information("Generating YAML..."));
}
