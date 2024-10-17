using FluentAssertions;
using NuGet.Frameworks;
using NuGet.Packaging;
using RestApia.Shared.Common;
using RestApia.Shared.Common.Enums;
using RestApia.Shared.Common.Services;
namespace RestApia.Experiments.Tests.Extensions;

public class ValidateMyExtension
{
    [TestCase(@"X:\projects\RestApia3\repos\RestApia.Shared\.local\builds\RestApia.Extensions.Import.Postman\RestApia.Extensions.Import.Postman.*.nupkg")]
    [TestCase(@"X:\projects\RestApia3\repos\RestApia.Shared\.local\builds\RestApia.Extensions.Auth.OAuth2\RestApia.Extensions.Auth.OAuth2.*.nupkg")]
    [TestCase(@"X:\projects\RestApia3\repos\RestApia.Shared\.local\builds\RestApia.Extensions.Auth.Basic\RestApia.Extensions.Auth.Basic.*.nupkg")]
    public void ValidateNugetPackage(string fileSearchPath)
    {
        var filePath = Directory.GetFiles(Directory.GetParent(fileSearchPath)!.FullName, Path.GetFileName(fileSearchPath)).FirstOrDefault();
        filePath.Should().NotBeEmpty();
        File.Exists(filePath).Should().BeTrue("Nuget package should exist");

        var targetFramework = NuGetFramework.ParseFrameworkName(".NETCoreApp,Version=v8.0", DefaultFrameworkNameProvider.Instance);
        using var package = new PackageArchiveReader(filePath);
        var packageIdentity = package.GetIdentity();

        // check identity
        packageIdentity.Id.Should().NotBeEmpty("Package should have an Id");
        packageIdentity.Version.Should().NotBeNull("Version should not be null");
        Console.WriteLine($"Package Id: {packageIdentity.Id}");
        Console.WriteLine($"Version: {packageIdentity.Version}");

        // search assembly
        var files = package
            .GetFiles()
            .Where(x =>
                x.StartsWith($"lib/{targetFramework.GetShortFolderName()}/", StringComparison.Ordinal) &&
                Path.GetFileName(x).Equals($"{packageIdentity.Id}.dll", StringComparison.Ordinal))
            .ToList();

        files.Should().NotBeEmpty($"Package should contain a '{packageIdentity.Id}.dll' file");
        files.Should().HaveCount(1, $"Package should contain only one '{packageIdentity.Id}.dll' file");
        Console.WriteLine($"Assembly found: {files[0]}");

        // check dependencies, must include shared library
        var dependencies = package.GetPackageDependencies().FirstOrDefault(x => NuGetFrameworkUtility.IsCompatibleWithFallbackCheck(targetFramework, x.TargetFramework));
        dependencies.Should().NotBeNull("Package should have dependencies");
        dependencies!.Packages.Should().NotBeEmpty("Package should have dependencies");

        var sharedLibrary = dependencies.Packages.FirstOrDefault(x => x.Id.Equals("RestApia.Shared", StringComparison.Ordinal));
        sharedLibrary.Should().NotBeNull("Package should have a dependency on 'RestApia.Shared'");

        // check metadata
        var owners = package.NuspecReader.GetOwners().IfEmpty(package.NuspecReader.GetAuthors());
        var description = package.NuspecReader.GetDescription();
        var projectUrl = package.NuspecReader.GetProjectUrl();

        owners.Should().NotBeEmpty("Package should have owners");
        description.Should().NotBeEmpty("Package should have a description");
        Console.WriteLine($"Description: {description}");
        Console.WriteLine($"Owners: {owners}");

        if (owners.Split(',').Select(x => x.Trim()).Any(x => x.Equals("RestApia", StringComparison.OrdinalIgnoreCase)))
            Console.WriteLine("[WARN] Package owners should not include 'RestApia'");

        if (projectUrl.IsEmpty()) Console.WriteLine("[WARN] It is recommended to have a project URL in the package metadata");
        else Console.WriteLine($"Project URL: {projectUrl}");

        // check requirements
        var requirements = PackageReader.RequirementsRead(package.NuspecReader.GetReleaseNotes());
        if (!requirements.TryGetValue(PackageRequirement.MinAppVersion, out var requirement)) Console.WriteLine("[WARN] It is recommended to have a 'MinAppVersion' requirement");
        else Console.WriteLine($"Minimum application version: {requirement}");
    }
}
