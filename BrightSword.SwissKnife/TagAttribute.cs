using System;

namespace BrightSword.SwissKnife;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
public class TagAttribute : Attribute
{
	public string Name { get; private set; }

	public object? Value { get; private set; }

	public TagAttribute(string name, object? value)
	{
		Name = name;
		Value = value;
	}
}
