#!/usr/bin/env bash
set -euo pipefail
OUTDIR="$1"
KEY="$2"
if [ -z "$KEY" ]; then
  echo "NUGET_API_KEY not provided, skipping publish"
  exit 0
fi
shopt -s nullglob
for p in "$OUTDIR"/*.nupkg; do
  echo "Pushing $p"
  dotnet nuget push "$p" -k "$KEY" -s https://api.nuget.org/v3/index.json || echo "Push failed for $p"
done
