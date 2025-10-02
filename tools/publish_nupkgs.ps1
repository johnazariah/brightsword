param(
  [string]$OutDir,
  [string]$Key
)
if ([string]::IsNullOrEmpty($Key)) {
  Write-Host "NUGET_API_KEY not provided, skipping publish"
  exit 0
}
Get-ChildItem -Path $OutDir -Filter *.nupkg -File | ForEach-Object {
  Write-Host "Pushing $($_.FullName)"
  try {
    dotnet nuget push $_.FullName -k $Key -s https://api.nuget.org/v3/index.json
  } catch {
    Write-Host "Push failed for $($_.FullName): $_"
  }
}
