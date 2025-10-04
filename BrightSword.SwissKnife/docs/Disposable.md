# Disposable

## Purpose
Wraps an instance and a dispose action, providing a simple <code>IDisposable</code> implementation. Useful for managing resources or cleanup actions in a scoped/disposable pattern.

## When to Use
- When you need to run cleanup logic or dispose actions in a <code>using</code> block.
- When you want to wrap an object and ensure a custom action is run on disposal.

## How to Use
Create a <code>Disposable<T></code> with an instance and a dispose action, and use it in a <code>using</code> block.

## Key APIs
- <code>Disposable<T>(T instance, Action<T> dispose)</code>: Wraps an instance and a dispose action.
- <code>Instance</code>: Gets the wrapped instance.
- <code>Dispose()</code>: Invokes the dispose action and suppresses finalization.

## Examples
```csharp
var resource = new Disposable<MyResource>(myResource, r => r.Cleanup());
using (resource) { /* use resource.Instance */ }
// Cleanup is called automatically on Dispose
```

## Remarks
These helpers reduce boilerplate for simple disposable actions and are useful in tests and short-lived scopes. They are especially useful for managing resources that do not implement <code>IDisposable</code> themselves.
