#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Increments the suite VersionPrefix in version.props.
.PARAMETER VersionFile
    Path to version.props.
.PARAMETER Level
    One of: Patch, Minor, Major. Defaults to Patch.
#>
param(
    [Parameter(Mandatory)]
    [string]$VersionFile,

    [ValidateSet('Patch', 'Minor', 'Major')]
    [string]$Level = 'Patch'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not (Test-Path $VersionFile)) {
    Write-Error "Version file not found: $VersionFile"
    exit 1
}

[xml]$xml = Get-Content $VersionFile -Raw
$node = $xml.SelectSingleNode('//VersionPrefix')
if (-not $node) {
    Write-Error "No <VersionPrefix> element found in $VersionFile"
    exit 1
}

$current = $node.InnerText.Trim()
$parts = $current.Split('.')
if ($parts.Length -ne 3) {
    Write-Error "VersionPrefix '$current' is not in Major.Minor.Patch format"
    exit 1
}

$major = [int]$parts[0]
$minor = [int]$parts[1]
$patch = [int]$parts[2]

switch ($Level.ToLower()) {
    'major' { $major++; $minor = 0; $patch = 0 }
    'minor' { $minor++; $patch = 0 }
    default { $patch++ }
}

$newVersion = "$major.$minor.$patch"
$node.InnerText = $newVersion

$settings = New-Object System.Xml.XmlWriterSettings
$settings.Indent = $true
$settings.IndentChars = '  '
$settings.OmitXmlDeclaration = $true
$settings.NewLineChars = "`n"
$settings.NewLineHandling = [System.Xml.NewLineHandling]::Replace

$writer = [System.Xml.XmlWriter]::Create($VersionFile, $settings)
try {
    $xml.Save($writer)
} finally {
    $writer.Dispose()
}

Write-Host "Suite version bumped: $current -> $newVersion"
