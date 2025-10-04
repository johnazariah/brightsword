#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Increment version numbers for BrightSword packages
.DESCRIPTION
    Updates the version in version.props files for specified packages
.PARAMETER Package
    The package to update (BrightSword.SwissKnife, BrightSword.Crucible, BrightSword.Feber, BrightSword.Squid, or All)
.PARAMETER Component
    Which version component to increment (Major, Minor, Patch). Default: Patch
.PARAMETER WhatIf
    Show what would be changed without making changes
.EXAMPLE
    .\increment-version.ps1 -Package BrightSword.SwissKnife
    Increment patch version of SwissKnife (e.g., 1.0.19 -> 1.0.20)
.EXAMPLE
    .\increment-version.ps1 -Package BrightSword.Feber -Component Minor
    Increment minor version of Feber (e.g., 2.0.3 -> 2.1.0)
.EXAMPLE
    .\increment-version.ps1 -Package All -Component Patch
    Increment patch version of all packages
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory)]
    [ValidateSet('BrightSword.SwissKnife', 'BrightSword.Crucible', 'BrightSword.Feber', 'BrightSword.Squid', 'All')]
    [string]$Package,

    [ValidateSet('Major', 'Minor', 'Patch')]
    [string]$Component = 'Patch',

    [switch]$WhatIf
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Update-VersionProps {
    param(
        [string]$ProjectName,
        [string]$VersionPropsPath,
        [string]$Component
    )

    if (-not (Test-Path $VersionPropsPath)) {
        Write-Warning "Version props file not found: $VersionPropsPath"
        return
    }

    # Load the XML
    [xml]$propsXml = Get-Content $VersionPropsPath -Raw

    # Find the VersionPrefix element
    $versionNode = $propsXml.Project.PropertyGroup.VersionPrefix
    if (-not $versionNode) {
        Write-Warning "VersionPrefix not found in $VersionPropsPath"
        return
    }

    $currentVersion = $versionNode
    Write-Host "Current version of ${ProjectName}: $currentVersion" -ForegroundColor Cyan

    # Parse version
    if ($currentVersion -match '^(\d+)\.(\d+)\.(\d+)$') {
        $major = [int]$Matches[1]
        $minor = [int]$Matches[2]
        $patch = [int]$Matches[3]

        # Increment based on component
        switch ($Component) {
            'Major' {
                $major++
                $minor = 0
                $patch = 0
            }
            'Minor' {
                $minor++
                $patch = 0
            }
            'Patch' {
                $patch++
            }
        }

        $newVersion = "$major.$minor.$patch"
        Write-Host "New version of ${ProjectName}: $newVersion" -ForegroundColor Green

        if (-not $WhatIf) {
            # Update the XML
            $propsXml.Project.PropertyGroup.VersionPrefix = $newVersion

            # Save with proper formatting
            $settings = New-Object System.Xml.XmlWriterSettings
            $settings.Indent = $true
            $settings.IndentChars = "  "
            $settings.NewLineChars = "`n"
            $settings.Encoding = [System.Text.UTF8Encoding]::new($false) # UTF-8 without BOM

            $writer = [System.Xml.XmlWriter]::Create($VersionPropsPath, $settings)
            try {
                $propsXml.Save($writer)
                Write-Host "? Updated $VersionPropsPath" -ForegroundColor Green
            }
            finally {
                $writer.Close()
            }
        }
        else {
            Write-Host "Would update $VersionPropsPath (WhatIf mode)" -ForegroundColor Yellow
        }
    }
    else {
        Write-Warning "Could not parse version: $currentVersion"
    }

    Write-Host ""
}

# Get script directory
$scriptRoot = $PSScriptRoot

# Define packages
$packages = @{
    'BrightSword.SwissKnife' = 'BrightSword.SwissKnife\version.props'
    'BrightSword.Crucible'   = 'BrightSword.Crucible\version.props'
    'BrightSword.Feber'      = 'BrightSword.Feber\version.props'
    'BrightSword.Squid'      = 'BrightSword.Squid\version.props'
}

Write-Host ""
Write-Host "BrightSword Version Increment Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Component: $Component" -ForegroundColor Yellow
if ($WhatIf) {
    Write-Host "Mode: WhatIf (no changes will be made)" -ForegroundColor Yellow
}
Write-Host ""

# Process packages
if ($Package -eq 'All') {
    foreach ($pkg in $packages.Keys) {
        $propsPath = Join-Path $scriptRoot $packages[$pkg]
        Update-VersionProps -ProjectName $pkg -VersionPropsPath $propsPath -Component $Component
    }
}
else {
    if ($packages.ContainsKey($Package)) {
        $propsPath = Join-Path $scriptRoot $packages[$Package]
        Update-VersionProps -ProjectName $Package -VersionPropsPath $propsPath -Component $Component
    }
    else {
        Write-Error "Unknown package: $Package"
    }
}

if (-not $WhatIf) {
    Write-Host "? Version increment completed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  1. Review the changes: git diff" -ForegroundColor Gray
    Write-Host "  2. Commit the version changes: git commit -am 'Bump version'" -ForegroundColor Gray
    Write-Host "  3. Tag the release: git tag v<version>" -ForegroundColor Gray
}
