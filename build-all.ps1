param(
    [string]$version = ""
)

# call script in cli
nuke Solution_Clean
nuke Shared_Build --SharedLibVersion $version

# find all in Extensions folder
$extensions = Get-ChildItem -Path .\src\Extensions -Directory
foreach ($extension in $extensions) {
    nuke Extension_Build --ExtensionName $extension.Name --ExtensionLibVersion $version
}

# find all nupkg files and store to one folder
$nugetsFolder = ".\.local\nuget"
if (Test-Path .\.local\nuget) {
    Remove-Item -Path $nugetsFolder -Recurse -Force
}
New-Item -ItemType Directory -Path $nugetsFolder -Force

$files = Get-ChildItem -Path .\.local\builds -Recurse -Filter *.nupkg
$files | ForEach-Object { Copy-Item $_.FullName -Destination $nugetsFolder -Force }