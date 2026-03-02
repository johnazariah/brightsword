using System;
using System.Diagnostics.CodeAnalysis;

namespace BrightSword.SwissKnife;

[ExcludeFromCodeCoverage]
public class CommandLineParameterException : Exception
{
	public CommandLineParameterException(string parameterName, string message)
		: base($"{parameterName} -- {message}")
	{
	}
}
