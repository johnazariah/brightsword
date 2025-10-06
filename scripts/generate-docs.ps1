<#
.SYNOPSIS
Docs generator: collects XML documentation files and project `docs/` markdown into `artifacts/docs`.

This script copies per-project docs into predictable folders under artifacts/docs/<ProjectName>/ and also copies top-level docs/*.md into artifacts/docs.
It produces a friendlier `index.html` by rendering the repository README.md (simple markdown -> HTML) and adding links to each project's documentation.

This updated version excludes the output directory from recursive scans to avoid copying generated artifacts back into the output.
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

    # Normalize full path for comparisons
    $fullOutDir = (Resolve-Path -Path $OutDir).ProviderPath

    # Copy XML docs produced by builds (exclude any files already in the OutDir)
    $xmls = Get-ChildItem -Recurse -File -Include *.xml | Where-Object { $_.FullName -like "*\bin\Release\net10.0\*.xml" -and $_.FullName -notlike "$fullOutDir*" }
    foreach ($x in $xmls) {
        $dest = Join-Path $OutDir $x.Name
        Copy-Item -Path $x.FullName -Destination $dest -Force
        Write-Host "Copied XML: $($x.FullName) -> $dest"
    }

    # Copy project docs into artifacts/docs/<ProjectName>/... (exclude scanning under OutDir)
    $mdDirs = Get-ChildItem -Recurse -Directory -Filter docs | Where-Object { $_.FullName -notmatch "\\obj\\|\\bin\\" -and $_.FullName -notlike "$fullOutDir*" }
    $projectDocs = @()
    foreach ($d in $mdDirs) {
        $projectDir = Split-Path $d.FullName -Parent
        $projName = Split-Path $projectDir -Leaf
        $target = Join-Path $OutDir $projName
        if (-not (Test-Path $target)) { New-Item -ItemType Directory -Path $target | Out-Null }
        Get-ChildItem -Path $d.FullName -Filter *.md -Recurse | Where-Object { $_.FullName -notlike "$fullOutDir*" } | ForEach-Object {
            $rel = $_.FullName.Substring($d.FullName.Length).TrimStart('\','/')
            $dest = Join-Path $target $rel
            $destDir = Split-Path $dest -Parent
            if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Path $destDir | Out-Null }
            Copy-Item -Path $_.FullName -Destination $dest -Force
            Write-Host "Copied MD: $($_.FullName) -> $dest"
        }
        # track project for index links
        $projectDocs += @{ Name = $projName; Readme = (Join-Path $target 'README.md') }
    }

    # Copy top-level docs/*.md into artifacts/docs root, but avoid copying already-generated files under OutDir
    $topDocsDir = Join-Path $Root 'docs'
    if (Test-Path $topDocsDir) {
        Get-ChildItem -Path $topDocsDir -Filter *.md -File | Where-Object { $_.FullName -notlike "$fullOutDir*" } | ForEach-Object {
            $dest = Join-Path $OutDir $_.Name
            Copy-Item -Path $_.FullName -Destination $dest -Force
            Write-Host "Copied top-level MD: $($_.FullName) -> $dest"
        }
    }

    # Helper: simple markdown to HTML converter (handles headings, lists, links, code fences, paragraphs)
    function Convert-MarkdownToHtml([string]$md) {
        if (-not $md) { return '' }
        # Normalize line endings
        $md = $md -replace "\r\n","\n"

        $lines = $md -split "\n"
        $inCode = $false
        $html = ""
        foreach ($line in $lines) {
            if ($line -match '^```') {
                if (-not $inCode) { $inCode = $true; $html += "<pre><code>"; continue } else { $inCode = $false; $html += "</code></pre>`n"; continue }
            }
            if ($inCode) { $html += [System.Net.WebUtility]::HtmlEncode($line) + "`n"; continue }

            # headings
            if ($line -match '^# (.*)') { $html += "<h1>" + [System.Net.WebUtility]::HtmlEncode($matches[1].Trim()) + "</h1>`n"; continue }
            if ($line -match '^## (.*)') { $html += "<h2>" + [System.Net.WebUtility]::HtmlEncode($matches[1].Trim()) + "</h2>`n"; continue }
            if ($line -match '^### (.*)') { $html += "<h3>" + [System.Net.WebUtility]::HtmlEncode($matches[1].Trim()) + "</h3>`n"; continue }

            # unordered list
            if ($line -match '^[\*\-\+]\s+(.*)') { 
                if ($html -notmatch '<ul[^>]*>$') { $html += "<ul>`n" }
                $item = [System.Net.WebUtility]::HtmlEncode($matches[1].Trim())
                # convert inline links [text](url)
                $item = [regex]::Replace($item, '\\[(.*?)\\]\\((.*?)\\)', '<a href="$2">$1</a>')
                $html += "<li>$item</li>`n"
                continue
            } else {
                if ($html -match '<ul[^>]*>$') { $html += "</ul>`n" }
            }

            # inline links and bold/italic (basic)
            $safe = [System.Net.WebUtility]::HtmlEncode($line)
            $safe = [regex]::Replace($safe, '\\[(.*?)\\]\\((.*?)\\)', '<a href="$2">$1</a>')
            $safe = $safe -replace '\*\*(.*?)\*\*', '<strong>$1</strong>'
            $safe = $safe -replace '\*(.*?)\*', '<em>$1</em>'

            if ($line -match '^\s*$') { $html += "<p></p>`n" } else { $html += "<p>$safe</p>`n" }
        }
        # close any open ul
        if ($html -match '<ul[^>]*>$') { $html += "</ul>`n" }
        return $html
    }

    # Build index.html: prefer root README.md if present
    $rootReadme = Join-Path $Root 'README.md'
    $indexHtml = Join-Path $OutDir 'index.html'

    $indexContent = "<html><head><meta charset='utf-8'><title>BrightSword Documentation</title><style>body{font-family:Segoe UI,Arial,Helvetica,sans-serif;max-width:900px;margin:40px auto;padding:0 20px;color:#222}header h1{margin:0} nav ul{list-style:none;padding:0} nav li{margin:6px 0}</style></head><body>"
    $indexContent += "<header><h1>BrightSword Documentation</h1><p>Documentation and API references for the BrightSword libraries.</p></header>"

    if (Test-Path $rootReadme) {
        Write-Host "Including root README.md in index"
        $md = Get-Content -Raw -Path $rootReadme
        $indexContent += "<section>"
        $indexContent += Convert-MarkdownToHtml $md
        $indexContent += "</section>"
    }

    # Add project links
    $indexContent += "<nav><h2>Project documentation</h2><ul>`n"
    foreach ($p in $projectDocs) {
        $name = $p.Name
        $readmeRel = "$name/README.md"
        # if README.md exists in artifacts for project, link it; otherwise link to project dir
        if (Test-Path (Join-Path $OutDir $readmeRel)) {
            $indexContent += "<li><a href='$readmeRel'>$name</a></li>`n"
        } else {
            $indexContent += "<li>$name (no README)</li>`n"
        }
    }
    # top-level docs
    if (Test-Path $topDocsDir) {
        Get-ChildItem -Path $topDocsDir -Filter *.md -File | ForEach-Object {
            $indexContent += "<li><a href='$_'>$($_.Name)</a></li>`n"
        }
    }
    $indexContent += "</ul></nav>"

    $indexContent += "<footer><p>Generated on: $(Get-Date -Format o)</p></footer></body></html>"
    [System.IO.File]::WriteAllText($indexHtml, $indexContent, [System.Text.Encoding]::UTF8)
    Write-Host "Wrote friendly index -> $indexHtml"

    Write-Host "Docs generation complete"
} finally {
    Pop-Location
}
