using System;

namespace BrightSword.SwissKnife;

public static class CommandLineProcessorExtensions
{
	[Obsolete("Create an instance of CommandLineArgumentHelper and use that instead!")]
	public static T GetArgumentValue<T>(this string[] args, string argumentName, Func<string?, T> valueConverter, T defaultValue = default!)
	{
		return new CommandLineArgumentHelper(args).GetArgumentValue(argumentName, valueConverter, defaultValue);
	}

	[Obsolete("Create an instance of CommandLineArgumentHelper and use that instead!")]
	public static bool HelpRequested(this string[] args)
	{
		return new CommandLineArgumentHelper(args).HelpRequested;
	}

	[Obsolete("Create an instance of CommandLineArgumentHelper and use that instead!")]
	public static bool IsArgumentSpecified(this string[] args, string argumentName)
	{
		return new CommandLineArgumentHelper(args).IsArgumentSpecified(argumentName);
	}
}
