# Getting Started

This guide helps you build and explore the BrightSword solution.

Prerequisites
- .NET 10 SDK (use the SDK pinned in `global.json` inside the repo root)
- PowerShell or your preferred terminal

Build the solution
1. Restore and build:
   - `dotnet restore BrightSword.sln`
   - `dotnet build BrightSword.sln`

Run tests
- Use dotnet test on the individual test projects, for example:
  - `dotnet test BrightSword.Squid.Tests/ BrightSword.Squid.Tests.csproj`

Explore the code
- `BrightSword.Feber` contains builder types that generate and cache delegates using expression trees.
- `BrightSword.SwissKnife` contains reusable utilities and extension methods.
- `BrightSword.Squid` contains runtime type emission helpers using `System.Reflection.Emit`.

Tips
- The repo uses `GenerateAssemblyInfo` = false in csproj files; avoid changing assembly version fields unintentionally.
- Run `dotnet format` to keep code style consistent.
