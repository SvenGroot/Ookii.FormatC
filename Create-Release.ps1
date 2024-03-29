param(
    [Parameter()][Switch]$PackageOnly,
    [Parameter()][string]$LocalNugetPath
)

# This script is used to create a distribution folder that can be packaged into a zip file for release.
$projects = @("Ookii.FormatC")
$publishDir = Join-Path $projects[0] "bin" "publish"
$zipDir = Join-Path $publishDir "zip"
New-Item $publishDir -ItemType Directory -Force | Out-Null
Remove-Item "$publishDir/*" -Recurse -Force
New-Item $zipDir -ItemType Directory -Force | Out-Null

[xml]$projectFile = Get-Content (Join-Path $PSScriptRoot "$($projects[0])/$($projects[0]).csproj")
$frameworks = ($projectFile.Project.PropertyGroup.TargetFrameworks | Where-Object { $null -ne $_ }) -split ";"
[xml]$props = Get-Content (Join-Path $PSScriptRoot "Directory.Build.Props")
$versionPrefix = $props.Project.PropertyGroup.VersionPrefix
$versionSuffix = $props.Project.PropertyGroup.VersionSuffix
if ($versionSuffix) {
    $version = "$versionPrefix-$versionSuffix"
} else {
    $version = $versionPrefix
}


# Clean and build deterministic.
dotnet clean "$PSScriptRoot" --configuration Release

# Build each project and framework separately. Without this, the nuget package almost always
# contains at least one pdb that isn't actually deterministic, even though we build with the
# ContinuousIntegrationBuild property. I don't really understand why, but this seems to work around
# it.
foreach ($project in $projects) {
    "Processing $project"
    foreach ($framework in $frameworks) {
        dotnet build "$PSScriptRoot/$project" --framework $framework --configuration Release /p:ContinuousIntegrationBuild=true
        # Uncomment the below to use mdv (https://github.com/dotnet/metadata-tools) to verify the
        # build was actually deterministic.
        # $sources = mdv "$PSScriptRoot/$project/bin/Release/$framework/$project.pdb" | Where-Object { $_.Contains("/_/src" ) }
        # if ($sources.Length -eq 0) {
        #     throw "$project, $framework is not deterministic"
        # }
    }

    "Creating package..."
    dotnet pack "$PSScriptRoot/$project" --no-build --configuration Release --output "$publishDir"
}

if (-not $PackageOnly) {
    # Publish each version of the library.
    foreach ($project in $projects) {
        foreach ($framework in $frameworks) {
            dotnet publish --no-build "$PSScriptRoot/$project" --configuration Release --framework $framework --output "$zipDir/$framework"
        }
    }

    # Copy global items.
    Copy-Item "$PSScriptRoot/license.txt" $zipDir
    Copy-Item "$PSScriptRoot/code.css" $zipDir
    Copy-Item "$PSScriptRoot/codedark.css" $zipDir
    Copy-Item "$PSScriptRoot/sample.html" $zipDir

    # Create readme.txt files.
    $url = "https://github.com/SvenGroot/Ookii.FormatC"
    "Ookii.FormatC $version","For documentation and other information, see:",$url | Set-Content "$zipDir/readme.txt"

    Compress-Archive "$zipDir/*" "$publishDir/Ookii.FormatC-$version.zip"
}

if ($LocalNugetPath) {
    Copy-Item "$publishDir\*.nupkg" $LocalNugetPath
    Copy-Item "$publishDir\*.snupkg" $LocalNugetPath
    Remove-Item -Recurse "~/.nuget/packages/ookii.formatc/*"
}
