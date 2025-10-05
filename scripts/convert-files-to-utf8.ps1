<#
.SYNOPSIS
Convert text files (.md, .json, .yml, .yaml) to UTF-8 encoding (no BOM) to avoid docfx/json parsing errors caused by non-UTF8 encodings.

Usage:
  pwsh ./scripts/convert-files-to-utf8.ps1

This script will process files under the repository root.
#>

param(
    [string]$Root = (Get-Location).Path
)

Write-Host "Converting files to UTF-8 under: $Root"
Push-Location $Root
try {
    $patterns = '*.md','*.json','*.yml','*.yaml'
    $files = Get-ChildItem -Recurse -File -Include $patterns -ErrorAction SilentlyContinue | Where-Object { $_.FullName -notmatch '\\bin\\|\\obj\\' }
    foreach ($f in $files) {
        try {
            # Read using the system default encoding to capture legacy files (Windows: cp1252). Use -Raw to preserve contents.
            $text = Get-Content -Raw -Encoding Default -Path $f.FullName -ErrorAction Stop
            # Write back as UTF8 without BOM
            $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
            [System.IO.File]::WriteAllText($f.FullName, $text, $utf8NoBom)
            Write-Host "Converted: $($f.FullName)" -ForegroundColor Green
        } catch {
            Write-Host "Skipped (could not read/convert): $($f.FullName) - $_" -ForegroundColor Yellow
        }
    }
} finally {
    Pop-Location
}
Write-Host "Conversion complete"
