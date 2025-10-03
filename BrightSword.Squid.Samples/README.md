# BrightSword.Squid.Samples

This sample project demonstrates the full lifecycle of using `BasicDataTransferObjectTypeCreator<T>`:
- Declare interfaces
- Configure a creator (facets, behaviours, TypeMaps)
- Create instances and inspect emitted types

See `Program.cs` for a runnable console sample.

## Running the sample

### Prerequisites
- .NET 10 SDK (the repository pins the SDK in `global.json`). Ensure you use the same SDK.

### Build and run
- From the repository root, build then run the sample:
  - `dotnet build BrightSword.Squid.Samples/BrightSword.Squid.Squid.Samples.csproj`
  - `dotnet run --project BrightSword.Squid.Samples/BrightSword.Squid.Squid.Samples.csproj`

The sample prints the emitted runtime type, sets properties on the generated instance and demonstrates the `Clone` behaviour when the facet interface is present.

### Notes
- The sample relies on runtime type emission (`Reflection.Emit`) and may behave differently on restricted or trimmed runtimes. Use a standard .NET SDK/runtime environment when running the sample.