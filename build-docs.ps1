#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build and preview documentation locally using DocFX
.DESCRIPTION
    This script installs DocFX (if needed) and builds the documentation site locally.
    You can preview the site before pushing to GitHub.
.PARAMETER Serve
    Start a local web server to preview the documentation
.PARAMETER Port
    Port number for the local web server (default: 8080)
.EXAMPLE
    .\build-docs.ps1
    Build documentation without starting server
.EXAMPLE
    .\build-docs.ps1 -Serve
    Build documentation and start local preview server
.EXAMPLE
    .\build-docs.ps1 -Serve -Port 9000
    Build and serve on port 9000
#>

[CmdletBinding()]
param(
    [switch]$Serve,
    [int]$Port = 8080
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host ""
Write-Host "BrightSword Documentation Builder" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

# Check if DocFX is installed
$docfxInstalled = $false
try {
    $null = Get-Command docfx -ErrorAction Stop
    $docfxInstalled = $true
    Write-Host "? DocFX is already installed" -ForegroundColor Green
}
catch {
    Write-Host "DocFX is not installed" -ForegroundColor Yellow
}

# Install DocFX if needed
if (-not $docfxInstalled) {
    Write-Host "Installing DocFX..." -ForegroundColor Yellow
    
    try {
        dotnet tool install -g docfx
        Write-Host "? DocFX installed successfully" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to install DocFX. Please install manually:" -ForegroundColor Red
        Write-Host "  dotnet tool install -g docfx" -ForegroundColor Yellow
        exit 1
    }
}

# Build documentation
Write-Host ""
Write-Host "Building documentation..." -ForegroundColor Yellow

try {
    docfx docfx.json
    Write-Host "? Documentation built successfully" -ForegroundColor Green
}
catch {
    Write-Host "Failed to build documentation" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Documentation output: _site/" -ForegroundColor Cyan

# Serve documentation if requested
if ($Serve) {
    Write-Host ""
    Write-Host "Starting documentation server on port $Port..." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Documentation will be available at:" -ForegroundColor Cyan
    Write-Host "  http://localhost:$Port" -ForegroundColor Green
    Write-Host ""
    Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
    Write-Host ""
    
    try {
        docfx serve _site -p $Port
    }
    catch {
        Write-Host "Failed to start server" -ForegroundColor Red
        Write-Host "Error: $_" -ForegroundColor Red
        exit 1
    }
}
else {
    Write-Host ""
    Write-Host "To preview the documentation locally, run:" -ForegroundColor Yellow
    Write-Host "  .\build-docs.ps1 -Serve" -ForegroundColor Cyan
    Write-Host ""
}
