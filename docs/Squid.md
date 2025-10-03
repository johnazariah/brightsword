# BrightSword.Squid

Squid provides runtime type emission utilities that create concrete DTO-like implementations from interfaces using `System.Reflection.Emit`.

Why use Squid
- Generate lightweight concrete implementations for interfaces at runtime without handwritten classes.
- Support facets (additional interfaces) and behaviours (e.g. clone, change-tracking) injected into emitted types.

Key Types
- `BasicDataTransferObjectTypeCreator<T>` — primary type used to generate a concrete implementation for `T` (usually an interface). It:
  - Scans interface members and produces backing fields, properties, events and methods.
  - Supports default-value initialization from `DefaultValueAttribute`.
  - Optionally tracks initialization state of get-only properties.
  - Allows behaviours to mutate the emitted type via `IBehaviour` implementations.

Usage
- Create a generator: `var creator = new BasicDataTransferObjectTypeCreator<IMyInterface>();`
- Create an instance: `var instance = creator.CreateInstance();`
- Or get the generated runtime `Type`: `var generatedType = typeof(BasicDataTransferObjectTypeCreator<IMyInterface>).Type;`

Extensibility
- Override protected members such as `PropertyOperations`, `MethodOperations`, and `ClassOperations` to add custom IL or attributes.
- Supply additional `FacetInterfaces` to have the emitted type implement extra interfaces.
- Provide `SpecialBehaviours` to apply pre-defined behaviours (e.g. `CloneBehaviour`).

Caveats
- Emitted types use `Reflection.Emit` and rely on runtime APIs that differ across platforms; persisting assemblies to disk may not be supported everywhere.
- Be cautious when changing the emitted method signatures or IL generation order — external code may depend on specific semantics.

Example
```csharp
var creator = new BasicDataTransferObjectTypeCreator<IMyDto>();
var dto = creator.CreateInstance();
// dto now implements IMyDto with default property values and standard get/set behavior.
```
