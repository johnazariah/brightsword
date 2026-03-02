using System;

namespace BrightSword.SwissKnife;

[AttributeUsage(AttributeTargets.Field)]
public class NameAttribute : Attribute
{
	public string Value { get; private set; }

	public NameAttribute(string value)
	{
		Value = value;
	}
}
