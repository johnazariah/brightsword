#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build script for BrightSword monorepo
.DESCRIPTION
    Provides convenient commands to build, test, and package the BrightSword projects
.PARAMETER Target
    The build target to execute (Build, Clean, Rebuild, Test, Pack, CI)
.PARAMETER Configuration
    The build configuration (Debug, Release). Default: Release
.PARAMETER Package
    Specific package to build/pack (BrightSword.SwissKnife, BrightSword.Feber, BrightSword.Squid)
.PARAMETER Verbosity
    MSBuild verbosity level (quiet, minimal, normal, detailed, diagnostic). Default: minimal
.EXAMPLE
    .\build.ps1
    Build all projects in Release configuration
.EXAMPLE
    .\build.ps1 -Target Pack
    Build and pack all packages
.EXAMPLE
    .\build.ps1 -Target Pack -Package BrightSword.SwissKnife
    Build and pack only the SwissKnife package
.EXAMPLE
    .\build.ps1 -Target Test
    Build and run all tests
.EXAMPLE
    .\build.ps1 -Target CI
    Run full CI build (clean, restore, build, test, pack)
#>

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('Build', 'Clean', 'Rebuild', 'Test', 'Pack', 'PackSingle', 'CI', 'Restore')]
    [string]$Target = 'Build',

    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$Package = '',

    [ValidateSet('quiet', 'minimal', 'normal', 'detailed', 'diagnostic')]
    [string]$Verbosity = 'minimal'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Get script directory
$scriptRoot = $PSScriptRoot
$buildProj = Join-Path $scriptRoot "Build.proj"

# Check if Build.proj exists
if (-not (Test-Path $buildProj)) {
    Write-Error "Build.proj not found at: $buildProj"
    exit 1
}

# Build MSBuild arguments
$msbuildArgs = @(
    $buildProj
    "/t:$Target"
    "/p:Configuration=$Configuration"
    "/v:$Verbosity"
    "/nologo"
)

# Add package parameter if specified
if ($Package) {
    $msbuildArgs += "/p:Package=$Package"
    if ($Target -eq 'Pack') {
        $msbuildArgs[1] = "/t:PackSingle"
    }
}

# Display what we're doing
Write-Host ""
Write-Host "BrightSword Build Script" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan
Write-Host "Target:        $Target" -ForegroundColor Yellow
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
if ($Package) {
    Write-Host "Package:       $Package" -ForegroundColor Yellow
}
Write-Host ""

# Execute MSBuild
try {
    Write-Host "Executing: msbuild $($msbuildArgs -join ' ')" -ForegroundColor Gray
    Write-Host ""
    
    & msbuild $msbuildArgs
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed with exit code $LASTEXITCODE"
        exit $LASTEXITCODE
    }
    
    Write-Host ""
    Write-Host "? Build completed successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Show output location for Pack target
    if ($Target -eq 'Pack' -or $Target -eq 'PackSingle' -or $Target -eq 'CI') {
        $packagesDir = Join-Path $scriptRoot "artifacts\packages"
        if (Test-Path $packagesDir) {
            Write-Host "Packages created in: $packagesDir" -ForegroundColor Cyan
            Get-ChildItem $packagesDir -Filter "*.nupkg" | ForEach-Object {
                Write-Host "  - $($_.Name)" -ForegroundColor Gray
            }
        }
    }
    
    # Show test results for Test target
    if ($Target -eq 'Test' -or $Target -eq 'CI') {
        $testResultsDir = Join-Path $scriptRoot "artifacts\test-results"
        if (Test-Path $testResultsDir) {
            Write-Host ""
            Write-Host "Test results in: $testResultsDir" -ForegroundColor Cyan
        }
    }
}
catch {
    Write-Error "Build failed: $_"
    exit 1
}
