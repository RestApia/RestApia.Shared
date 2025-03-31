param(
    [string]$version = ""
)

# call script in cli
nuke Solution_Clean
nuke Shared_Push --SharedLibVersion $version

# find all in Extensions folder
$extensions = Get-ChildItem -Path .\src\Extensions -Directory
#$extensions = @(
#    [PSCustomObject]@{ Name = "RestApia.Extensions.ValuesProvider.AzureKeyVault" },
#    [PSCustomObject]@{ Name = "RestApia.Extensions.ValuesProvider.CollectionValuesProvider" }
#)

foreach ($extension in $extensions) {
    nuke Extension_Push --ExtensionName $extension.Name --ExtensionLibVersion $version
}