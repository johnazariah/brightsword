<#
.SYNOPSIS
Generate package-dependencies.json mapping package IDs to their project reference dependencies.

Usage:
  pwsh ./scripts/generate-package-dependencies.ps1

Outputs:
  package-dependencies.json at repository root containing:
  {
    "dependencies": { "PackageA": ["PackageB", "PackageC"] , ... },
    "dependents":  { "PackageB": ["PackageA"], ... },
    "dependentsRecursive": { "PackageB": ["PackageA","PackageD"], ... },
    "publishOrder": { "PackageA": ["PackageA","PackageB","PackageC"], ... }
  }
#>

param(
    [string]$Root = (Get-Location).Path,
    [string]$VersionProps = "version.props",
    [string]$Output = "package-dependencies.json"
)

Set-StrictMode -Version Latest

Push-Location $Root
try {
    Write-Host "Scanning for .csproj files under: $Root"
    $csprojFiles = Get-ChildItem -Recurse -Filter *.csproj | Where-Object { -not $_.FullName.Contains("obj") -and -not $_.FullName.Contains("bin") }

    if (-not $csprojFiles) {
        Write-Error "No .csproj files found."
        exit 1
    }

    # Load version.props if present
    $versionDoc = $null
    if (Test-Path $VersionProps) {
        try { $versionDoc = [xml](Get-Content $VersionProps -Raw) } catch { $versionDoc = $null }
    }

    # First pass: collect package id for each project
    $projects = @{}
    foreach ($f in $csprojFiles) {
        $full = $f.FullName
        $projName = [System.IO.Path]::GetFileNameWithoutExtension($full)
        $xml = $null
        try { $xml = [xml](Get-Content $full -Raw) } catch { $xml = $null }

        $packageId = $null
        if ($xml) {
            # Try to find explicit PackageId in the csproj
            $node = $xml.SelectSingleNode("/*[local-name()='Project']/*[local-name()='PropertyGroup']/*[local-name()='PackageId']")
            if ($node -and $node.InnerText.Trim() -ne '') { $packageId = $node.InnerText.Trim() }
            # Try AssemblyName as fallback
            if (-not $packageId) {
                $an = $xml.SelectSingleNode("/*[local-name()='Project']/*[local-name()='PropertyGroup']/*[local-name()='AssemblyName']")
                if ($an -and $an.InnerText.Trim() -ne '') { $packageId = $an.InnerText.Trim() }
            }
        }

        # If still not found, search version.props for conditioned PropertyGroup
        if (-not $packageId -and $versionDoc) {
            foreach ($pg in $versionDoc.Project.PropertyGroup) {
                $cond = $pg.GetAttribute('Condition')
                if ($cond -and $cond.Contains("'$projName'")) {
                    $pidNode = $pg.PackageId
                    if ($pidNode -and $pidNode.Trim() -ne '') { $packageId = $pidNode.Trim() }
                    break
                }
            }
        }

        if (-not $packageId) {
            # final fallback to project filename
            $packageId = $projName
        }

        $projects[$full] = [PSCustomObject]@{
            ProjectName = $projName
            PackageId = $packageId
            FilePath = $full
            Directory = $f.DirectoryName
        }
    }

    # Second pass: compute dependencies by resolving ProjectReference includes
    $dependencies = @{}

    foreach ($kv in $projects.GetEnumerator()) {
        $p = $kv.Value
        $file = $p.FilePath
        $xml = $null
        try { $xml = [xml](Get-Content $file -Raw) } catch { $xml = $null }

        $depList = @()
        if ($xml) {
            $refs = $xml.SelectNodes("/*[local-name()='Project']/*[local-name()='ItemGroup']/*[local-name()='ProjectReference']")
            if ($refs) {
                foreach ($r in $refs) {
                    $inc = $r.Include
                    if (-not $inc) { continue }
                    $refPath = [System.IO.Path]::GetFullPath((Join-Path $p.Directory $inc))
                    if ($projects.ContainsKey($refPath)) {
                        $depList += $projects[$refPath].PackageId
                    } else {
                        # If referenced project is outside collected set, attempt to infer package id from filename
                        $refName = [System.IO.Path]::GetFileNameWithoutExtension($inc)
                        $depList += $refName
                    }
                }
            }
        }

        $dependencies[$p.PackageId] = ($depList | Select-Object -Unique)
    }

    # Build reverse mapping (dependents)
    $dependents = @{
    }
    foreach ($pkg in $dependencies.Keys) { $dependents[$pkg] = @() }
    foreach ($kv in $dependencies.GetEnumerator()) {
        $pkg = $kv.Key
        foreach ($d in $kv.Value) {
            if (-not $dependents.ContainsKey($d)) { $dependents[$d] = @() }
            $dependents[$d] += $pkg
        }
    }

    # Compute recursive dependents (DFS)
    function Get-RecursiveDependents([string]$pkg, [hashtable]$dependentsMap) {
        # Use plain PowerShell collections to maximize compatibility across pwsh versions
        $seen = @{}
        $stack = @()
        if ($dependentsMap.ContainsKey($pkg)) {
            foreach ($d in $dependentsMap[$pkg]) { $stack += $d }
        }

        while ($stack.Count -gt 0) {
            $idx = $stack.Count - 1
            $cur = $stack[$idx]
            if ($idx -eq 0) { $stack = @() } else { $stack = $stack[0..($idx - 1)] }

            if (-not $seen.ContainsKey($cur)) {
                $seen[$cur] = $true
                if ($dependentsMap.ContainsKey($cur)) {
                    foreach ($n in $dependentsMap[$cur]) { $stack += $n }
                }
            }
        }

        return $seen.Keys
    }

    $dependentsRecursive = @{
    }
    foreach ($pkg in $dependencies.Keys) {
        $rec = Get-RecursiveDependents -pkg $pkg -dependentsMap $dependents
        # ensure deterministic ordering
        $dependentsRecursive[$pkg] = ($rec | Sort-Object)
    }

    # Helper: topological sort (Kahn's algorithm) for a subgraph defined by node set
    function TopologicalSort([string[]]$nodes, [hashtable]$dependenciesMap) {
        $nodesSet = [System.Collections.Generic.HashSet[string]]::new()
        foreach ($n in $nodes) { [void]$nodesSet.Add($n) }

        # compute indegrees within subgraph
        $indegree = @{
        }
        foreach ($n in $nodes) { $indegree[$n] = 0 }
        foreach ($n in $nodes) {
            $deps = @()
            if ($dependenciesMap.ContainsKey($n)) { $deps = $dependenciesMap[$n] }
            foreach ($d in $deps) {
                if ($nodesSet.Contains($d)) { $indegree[$d] = $indegree[$d] + 1 }
            }
        }

        $queue = New-Object System.Collections.Generic.Queue[string]
        foreach ($n in $nodes) { if ($indegree[$n] -eq 0) { $queue.Enqueue($n) } }

        $order = @()
        while ($queue.Count -gt 0) {
            $n = $queue.Dequeue()
            $order += $n
            $deps = @()
            if ($dependenciesMap.ContainsKey($n)) { $deps = $dependenciesMap[$n] }
            foreach ($m in $deps) {
                if ($nodesSet.Contains($m)) {
                    $indegree[$m] = $indegree[$m] - 1
                    if ($indegree[$m] -eq 0) { $queue.Enqueue($m) }
                }
            }
        }

        # If not all nodes included, graph has a cycle; append remaining nodes deterministically
        if ($order.Count -ne $nodes.Count) {
            $remaining = $nodes | Where-Object { $order -notcontains $_ } | Sort-Object
            $order += $remaining
        }
        return $order
    }

    # Compute publish order per package: subgraph nodes = package + recursive dependents; topologically sort so dependencies come before dependents
    $publishOrder = @{
    }
    foreach ($pkg in $dependencies.Keys) {
        $nodes = @($pkg) + $dependentsRecursive[$pkg]
        # include unique and deterministic ordering
        $nodes = $nodes | Select-Object -Unique | Sort-Object
        # topological sort expects dependencies mapping where keys are nodes and values are their dependencies
        $order = TopologicalSort -nodes $nodes -dependenciesMap $dependencies
        $publishOrder[$pkg] = $order
    }

    # Normalize arrays
    $out = [PSCustomObject]@{
        generated = (Get-Date).ToString("o")
        dependencies = (ConvertTo-Json $dependencies -Depth 10 | ConvertFrom-Json)
        dependents = (ConvertTo-Json $dependents -Depth 10 | ConvertFrom-Json)
        dependentsRecursive = (ConvertTo-Json $dependentsRecursive -Depth 10 | ConvertFrom-Json)
        publishOrder = (ConvertTo-Json $publishOrder -Depth 10 | ConvertFrom-Json)
    }

    $json = $out | ConvertTo-Json -Depth 10
    $json | Out-File -FilePath $Output -Encoding UTF8
    Write-Host "Wrote $Output"
    exit 0
}
finally {
    Pop-Location
}
