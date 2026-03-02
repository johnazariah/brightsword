using System;
using System.Collections.Generic;
using System.Linq;

namespace BrightSword.SwissKnife;

public class CommandLineArgumentHelper
{
	private readonly IDictionary<string, string?> _commandLineArguments;

	public bool HelpRequested => _commandLineArguments.ContainsKey("help") || _commandLineArguments.ContainsKey("usage");

	public int Count => _commandLineArguments.Count;

	public string? this[string argumentName]
	{
		get
		{
			if (!_commandLineArguments.ContainsKey(argumentName))
			{
				throw new ArgumentOutOfRangeException("argumentName", "No such argument specified!");
			}
			return _commandLineArguments[argumentName];
		}
	}

	public CommandLineArgumentHelper(params string[] args)
	{
		var source = from _arg in args
			let parts = _arg.Split('=')
			let name = parts[0].StartsWith("--") ? parts[0].Substring("--".Length) : parts[0]
			let value = (parts.Length > 1) ? parts[1].Trim() : null
			select new
			{
				Name = name,
				Value = value
			};
		_commandLineArguments = source.ToDictionary(_ => _.Name, _ => _.Value, StringComparer.InvariantCultureIgnoreCase);
	}

	public T GetArgumentValue<T>(string argumentName, Func<string?, T> valueConverter, T defaultValue = default!)
	{
		return _commandLineArguments.TryGetValue(argumentName, out string? value) ? valueConverter(value) : defaultValue;
	}

	public bool IsArgumentSpecified(string argumentName)
	{
		return _commandLineArguments.ContainsKey(argumentName);
	}
}
