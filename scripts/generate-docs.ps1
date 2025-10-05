<#
.SYNOPSIS
Placeholder docs generator: collects XML documentation files and project `docs/` markdown into `artifacts/docs`.

This script is a lightweight no-op generator suitable for CI when a full DocFX/MkDocs pipeline is not configured.
#>

param(
    [string]$Root = (Get-Location).Path,
    [string]$OutDir = "artifacts/docs"
)

Set-StrictMode -Version Latest

Push-Location $Root
try {
    Write-Host "Generating docs into: $OutDir"
    if (-Not (Test-Path $OutDir)) { New-Item -ItemType Directory -Path $OutDir | Out-Null }

    # Copy XML docs produced by builds
    $xmls = Get-ChildItem -Recurse -File -Include *.xml | Where-Object { $_.FullName -like "*\bin\Release\net10.0\*.xml" }
    foreach ($x in $xmls) {
        $dest = Join-Path $OutDir $x.Name
        Copy-Item -Path $x.FullName -Destination $dest -Force
        Write-Host "Copied XML: $($x.FullName) -> $dest"
    }

    # Copy project docs
    $mdDirs = Get-ChildItem -Recurse -Directory -Filter docs | Where-Object { $_.FullName -notmatch "\\obj\\|\\bin\\" }
    foreach ($d in $mdDirs) {
        $target = Join-Path $OutDir ($d.FullName.Replace((Get-Location).Path,'').TrimStart('\','/').Replace('\','-').Replace('/','-'))
        New-Item -ItemType Directory -Path $target -Force | Out-Null
        Get-ChildItem -Path $d.FullName -Filter *.md -Recurse | ForEach-Object {
            $rel = $_.FullName.Substring($d.FullName.Length).TrimStart('\','/')
            $dest = Join-Path $target $rel
            $destDir = Split-Path $dest -Parent
            if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Path $destDir | Out-Null }
            Copy-Item -Path $_.FullName -Destination $dest -Force
            Write-Host "Copied MD: $($_.FullName) -> $dest"
        }
    }

    # Create a minimal index.html listing files
    $index = Join-Path $OutDir "index.html"
    $items = Get-ChildItem -Recurse -File -Path $OutDir | Where-Object { $_.Name -ne 'index.html' }
    $html = "<html><head><meta charset='utf-8'><title>BrightSword Docs</title></head><body><h1>BrightSword Docs</h1><ul>`n"
    foreach ($it in $items) {
        $rel = $it.FullName.Substring((Get-Location).Path.Length).TrimStart('\','/')
        $link = $it.FullName.Substring((Get-Location).Path.Length).TrimStart('\','/').Replace('\\','/')
        $html += "<li><a href='$link'>$($it.Name)</a></li>`n"
    }
    $html += "</ul></body></html>"
    [System.IO.File]::WriteAllText($index, $html, [System.Text.Encoding]::UTF8)
    Write-Host "Wrote index: $index"

    Write-Host "Docs generation complete"
} finally {
    Pop-Location
}
