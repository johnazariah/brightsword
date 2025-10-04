# AttributeExtensions

## Purpose
Provides extension methods for retrieving custom attributes and their values from types and members. These helpers simplify attribute discovery and value extraction for reflection scenarios.

## When to Use
- When you need to retrieve custom attributes from types or members using reflection.
- When you want to extract values from attributes in a null-safe and convenient way.

## How to Use
Use these methods to get single or multiple attributes, or extract values from attributes using a selector function.

## Key APIs
- `GetCustomAttribute<TAttribute>(this Type @this)`: Gets the first custom attribute of the specified type applied to a type.
- `GetCustomAttribute<TAttribute>(this MemberInfo @this)`: Gets the first custom attribute of the specified type applied to a member.
- `GetCustomAttributeValue<TAttribute, TResult>(this Type @this, Func<TAttribute, TResult> selector, TResult defaultValue = default)`: Gets a value from the first custom attribute of the specified type applied to a type.
- `GetCustomAttributeValue<TAttribute, TResult>(this MemberInfo @this, Func<TAttribute, TResult> selector, TResult defaultValue = default)`: Gets a value from the first custom attribute of the specified type applied to a member.

## Examples
```csharp
// Get a single attribute from a type
var attr = typeof(MyClass).GetCustomAttribute<SerializableAttribute>();

// Get a value from an attribute
var message = typeof(MyClass).GetCustomAttributeValue<SerializableAttribute, string>(attr => "Serializable", "NotSerializable");

// Get a single attribute from a member
var method = typeof(MyClass).GetMethod("OldMethod");
var obsolete = method.GetCustomAttribute<ObsoleteAttribute>();

// Get a value from an attribute on a member
var msg = method.GetCustomAttributeValue<ObsoleteAttribute, string>(attr => attr.Message, "");
```

## Remarks
These are convenience wrappers over System.Reflection that reduce casting/boilerplate when working with attributes. They return null or default values where appropriate to keep call sites simple and safe.
