# FastMapper

Purpose
- Collection of mapping helpers showcasing static-to-static, static-to-dynamic, dynamic-to-static and dynamic-to-dynamic mapping. Uses `DynamicExpressionUtilities` for dynamic access/mutation.

Copyable examples
```csharp
// Static -> Static
var src = new Person { Name = "Ada", Age = 30 };
var dst = new Person();
dst.MapStaticToStatic(src);

// Dynamic -> Static (ExpandoObject -> typed person)
dynamic dyn = new System.Dynamic.ExpandoObject();
dyn.Name = "Ada";
dyn.Age = 30;
var person = new Person();
person.MapDynamicToStatic(dyn);
```

Backing-field mapping (detailed)

Some types expose read-only properties that are backed by private fields (for example an immutable property that gets its value from a private `_name` field). The `FastMapper` sample includes a mapper that assigns to conventional backing fields when writing to such types from a dynamic source.

How it works
- The `DynamicToStaticBackingFieldsMapperBuilder` override inspects each prototype property and looks for a corresponding non-public instance field using a conventional naming pattern:

	`_` + lowercased-first-letter + remainder-of-name

	For example, a property `Name` is expected to have a backing field named `_name`.

- If such a field exists, the builder emits an `Expression.Assign(Expression.Field(leftInstanceParameter, field), dynamicAccessor)` expression. The `dynamicAccessor` is produced by `DynamicExpressionUtilities.GetDynamicPropertyAccessorExpression<T>` which reads the property value from the dynamic source.

Copyable backing-field example
```csharp
public class Person
{
		private string _name; // backing field
		public string Name => _name; // read-only property
		public int Age { get; private set; }

		public override string ToString() => $"Name={Name}, Age={Age}";
}

// Usage:
dynamic dyn = new System.Dynamic.ExpandoObject();
dyn.Name = "Ada";
dyn.Age = 30;

var p = new Person();
// This will assign to the private backing field _name and to Age's setter (if available)
p.MapToBackingFields(dyn);

Console.WriteLine(p); // Name=Ada, Age=30
```


Backing-field naming convention (exact behavior)

- The backing-field mapper uses a simple, deterministic convention to locate candidate private fields for a given property. The field name is constructed like this in the code:

	1. Take the property name, e.g. `Name`.
	2. Lower-case only the first character using the current culture (the code calls `ToLower(System.Globalization.CultureInfo.CurrentCulture)` on the first character).
	3. Prefix the result with a single underscore (`_`).

	So the computed field name is: `"_" + char.ToLower(propertyName[0], CultureInfo.CurrentCulture) + propertyName.Substring(1)`

Examples
- `Name` -> `_name`
- `Age` -> `_age`
- `IPAddress` -> `_iPAddress` (note: only the first character is lower-cased)

Caveats and recommendations
- This is a convention-based lookup only; it will not detect compiler-generated auto-property backing fields (which commonly look like `<PropertyName>k__BackingField`) unless the mapper is extended to check for those names explicitly.
- The current implementation lower-cases the first character using the current culture. If you need culture-invariant behavior, change the implementation to use `CultureInfo.InvariantCulture` when lower-casing.
- Assigning into private fields circumvents the type's encapsulation â€” exercise caution and prefer using setters when available.
- If no backing field or setter is available, the mapper leaves the property unchanged (no-op) for that property.

Possible improvements (optional)
- Extend the mapper to also check for compiler-generated backing fields like `<PropertyName>k__BackingField` (use `BindingFlags.NonPublic` and search for that exact name), and/or perform a case-insensitive search for candidate fields.
- Expose a hook or strategy delegate so callers can customize how backing fields are discovered (e.g., a Func<PropertyInfo,string> that returns the field name to look up).

