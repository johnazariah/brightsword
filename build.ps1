<#
.SYNOPSIS
  Helper script to run common build/publish MSBuild targets on Windows.

.DESCRIPTION
  This script wraps calls to `dotnet msbuild build.proj` making it easier for
  Windows maintainers to run the same targets used by CI.

.PARAMETER Target
  MSBuild target to invoke (e.g. PublishSwissknife, PublishFeber, PackSwissKnife).

.PARAMETER Commit
  If supplied, causes MSBuild to run with DoCommit=true (attempts to commit bumped files).

.PARAMETER NUGET_API_KEY
  Optional NuGet API key to pass through to MSBuild for publishing.

.PARAMETER VersionFromTag
  Optional explicit version to pass to the bump script (used for tag-driven publishes).

.EXAMPLE
  .\build.ps1 -Target PackSwissKnife

.EXAMPLE
  .\build.ps1 -Target PublishSwissknife -Commit -NUGET_API_KEY $env:NUGET_API_KEY

#>

param(
    [string]$Target = 'PackSwissKnife',
    [switch]$Commit,
    [string]$NUGET_API_KEY = '',
    [string]$VersionFromTag = '',
    [switch]$Help
)

if ($Help) {
    Get-Help -Full $MyInvocation.MyCommand.Path
    exit 0
}

Write-Host "Running build wrapper: Target=$Target Commit=$Commit VersionFromTag=$VersionFromTag"

$DoCommit = if ($Commit) { 'true' } else { 'false' }

$props = @('-p:DoCommit=' + $DoCommit)
if ($NUGET_API_KEY -ne '') { $props += ('-p:NUGET_API_KEY=' + $NUGET_API_KEY) }
if ($VersionFromTag -ne '') { $props += ('-p:VersionFromTag=' + $VersionFromTag) }

$buildArgs = @('-t:' + $Target) + $props

$cmd = 'dotnet msbuild build.proj ' + ($buildArgs -join ' ')
Write-Host "Invoking: $cmd"

& dotnet msbuild build.proj @($buildArgs)
$rc = $LASTEXITCODE
if ($rc -ne 0) {
    Write-Error "msbuild returned exit code $rc"
    exit $rc
}

Write-Host "Done."
