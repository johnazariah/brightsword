using System;
using System.Diagnostics.CodeAnalysis;

namespace BrightSword.SwissKnife;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
[ExcludeFromCodeCoverage]
public sealed class CommandLineApplicationAttribute : Attribute
{
	public string? Description { get; set; }

	public CommandLineApplicationAttribute(string? description = null)
	{
		Description = description;
	}
}
