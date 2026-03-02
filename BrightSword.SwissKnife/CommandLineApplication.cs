using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife;

[ExcludeFromCodeCoverage]
[Obsolete("Review and rewrite using dynamic and generics!")]
public static class CommandLineApplication
{
	internal static class CommandLineApplicationClosure<TCommandLineParams>
	{
		private const BindingFlags C_BINDING_FLAGS = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		internal static TCommandLineParams Parameters { get; set; }

		internal static CommandLineApplicationAttribute? ApplicationAttribute { get; set; }

		internal static IDictionary<string, CommandLineArgumentAttribute> DefaultParameterTemplates { get; set; }

		internal static IDictionary<string, CommandLineArgumentAttribute> ParameterTemplates { get; set; }

		internal static string ParameterNames { get; set; }

		internal static string Name { get; set; }

		internal static bool Verbose { get; set; }

		internal static string Description { get; set; }

		static CommandLineApplicationClosure()
		{
			Parameters = (TCommandLineParams)Activator.CreateInstance(typeof(TCommandLineParams))!;
			ApplicationAttribute = typeof(TCommandLineParams).GetCustomAttribute<CommandLineApplicationAttribute>();
			DefaultParameterTemplates = new Dictionary<string, CommandLineArgumentAttribute>
			{
				{
					"help",
					new CommandLineArgumentAttribute("help", "Prints this message and quits", flag: true)
				},
				{
					"verbose",
					new CommandLineArgumentAttribute("verbose", "Emits more information", flag: true)
				}
			};
			ParameterTemplates = BuildParameterTemplates();
			ParameterNames = BuildParameterNames();
			Name = new FileInfo(Environment.GetCommandLineArgs()[0]).Name;
			Description = ((ApplicationAttribute == null || string.IsNullOrEmpty(ApplicationAttribute.Description)) ? "A cool but nondescript application" : ApplicationAttribute.Description);
			ApplyDefaultValues();
		}

		public static TCommandLineParams ProcessArguments(IEnumerable<string> args)
		{
			IDictionary<string, string?> dictionary = BuildArgumentPairs(args);
			if (dictionary.ContainsKey("help"))
			{
				Usage();
				throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
			}
			Verbose = dictionary.ContainsKey("verbose");
			try
			{
				SetParameterValues(dictionary, Parameters);
			}
			catch (Exception exception)
			{
				Error(exception, null, -1);
				throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
			}
			if (!ValidateSuccessful())
			{
				Error(null, "Validation failed", -1);
				throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
			}
			if (Verbose)
			{
				VerboseReport();
			}
			return Parameters;
		}

		internal static IDictionary<string, CommandLineArgumentAttribute> BuildParameterTemplates()
		{
			Dictionary<string, CommandLineArgumentAttribute> dictionary = new Dictionary<string, CommandLineArgumentAttribute>(DefaultParameterTemplates);
			PropertyInfo[] properties = typeof(TCommandLineParams).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			IEnumerable<CommandLineArgumentAttribute?> source = properties.Select((PropertyInfo _mi) => _mi.GetCustomAttributeValue(delegate(CommandLineArgumentAttribute _)
			{
				_.PropertyName = _mi.Name;
				_.ParamType = _mi.PropertyType;
				return _;
			}));
			foreach (CommandLineArgumentAttribute item in source.Where((CommandLineArgumentAttribute? _) => _ != null)!)
			{
				dictionary.Add(item.Name, item);
			}
			FieldInfo[] fields = typeof(TCommandLineParams).GetFields(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			IEnumerable<CommandLineArgumentAttribute?> source2 = fields.Select((FieldInfo _mi) => _mi.GetCustomAttributeValue(delegate(CommandLineArgumentAttribute _)
			{
				_.PropertyName = _mi.Name;
				_.ParamType = _mi.FieldType;
				return _;
			}));
			foreach (CommandLineArgumentAttribute item2 in source2.Where((CommandLineArgumentAttribute? _) => _ != null)!)
			{
				dictionary.Add(item2.Name, item2);
			}
			return dictionary;
		}

		internal static string BuildParameterNames()
		{
			IEnumerable<string> source = ParameterTemplates.Values.Select((CommandLineArgumentAttribute _) => _.ArgumentDescriptor);
			return source.Aggregate(string.Empty, (string _r, string _c) => $"{_r} {_c}");
		}

		internal static void ApplyDefaultValues()
		{
			foreach (CommandLineArgumentAttribute value in ParameterTemplates.Values)
			{
				value.SetValue(Parameters, value.DefaultValue, fSettingDefault: true);
			}
		}

		internal static IDictionary<string, string?> BuildArgumentPairs(IEnumerable<string> args)
		{
			Dictionary<string, string?> dictionary = new Dictionary<string, string?>();
			foreach (string arg in args)
			{
				string[] array = arg.Split('=');
				string key = array[0].Replace("--", "").ToLower();
				string? value = ((array.Length > 1) ? array[1] : null);
				dictionary[key] = value;
			}
			return dictionary;
		}

		internal static void SetParameterValues(IEnumerable<KeyValuePair<string, string?>> arguments, TCommandLineParams commandLineParams)
		{
			foreach (KeyValuePair<string, string?> item in arguments.Where((KeyValuePair<string, string?> _) => ParameterTemplates.ContainsKey(_.Key)))
			{
				ParameterTemplates[item.Key].SetValue(commandLineParams, item.Value);
			}
		}

		internal static bool ValidateSuccessful()
		{
			return !(from claa in ParameterTemplates.Values
				let value = claa.GetValue(Parameters)
				let found = value != null && !string.IsNullOrEmpty(value as string)
				where !claa.IsOptional && !claa.IsFlag && !found
				select claa).Any();
		}

		internal static void VerboseReport()
		{
			Console.WriteLine("***********************************************************************************************");
			Console.WriteLine("***");
			Console.WriteLine("*** {0} - {1}", Name, Description);
			Console.WriteLine("***");
			Console.WriteLine("*** Run Started: {0}", DateTime.Now);
			Console.WriteLine("***");
			foreach (CommandLineArgumentAttribute value in ParameterTemplates.Values)
			{
				Console.WriteLine("***\t\t The effective value of --{0} is [{1}]", value.Name, value.GetValue(Parameters) ?? "null");
			}
			Console.WriteLine("***");
			Console.WriteLine("***********************************************************************************************");
		}

		internal static void Error(Exception? exception = null, string? error = null, int errorCode = 0)
		{
			string? text = ((exception == null) ? null : (" : " + exception.Message));
			string arg = text ?? error ?? "Unknown error";
			Console.WriteLine("***********************************************************************************************");
			Console.WriteLine("***");
			Console.WriteLine("*** {0} - {1}", Name, Description);
			Console.WriteLine("***");
			Console.WriteLine("*** ERROR");
			Console.WriteLine("*** ERROR {0}", arg);
			Console.WriteLine("*** ERROR");
			Console.WriteLine("***");
			Console.WriteLine("***");
			Console.WriteLine("*** ABORTING");
			Console.WriteLine("***");
			PrintUsageMessage();
			Console.WriteLine("***");
			Console.WriteLine("***********************************************************************************************");
			Environment.Exit(errorCode);
		}

		internal static void PrintUsageMessage()
		{
			Console.WriteLine("*** Usage: {0} {1}", Name, ParameterNames);
			Console.WriteLine("***");
			foreach (CommandLineArgumentAttribute value in ParameterTemplates.Values)
			{
				string arg = (value.IsOptional ? "[Optional] " : "");
				Console.WriteLine("***\t {0,-18} : {1}{2}", value.ArgumentDescriptor, arg, value.Description);
				if (value.ParamType != null && value.ParamType.IsEnum)
				{
					Console.WriteLine("***\t\t --{0} should be one of [{1}]", value.Name, string.Join(", ", Enum.GetNames(value.ParamType)));
				}
				if (value.DefaultValue != null)
				{
					Console.WriteLine("***\t\t --{0} has a default value of [{1}]", value.Name, value.DefaultValue);
				}
				Console.WriteLine("***\t\t The effective value of --{0} is [{1}]", value.Name, value.GetValue(Parameters) ?? "null");
				Console.WriteLine("***");
			}
			Console.WriteLine("***");
			Console.WriteLine("*** Ensure that there are no spaces except between arguments.");
			Console.WriteLine("*** Quote values that embed a space.");
			Console.WriteLine("*** Use '--arg=value' and not '--arg = value'.");
			Console.WriteLine("***");
			Console.WriteLine("*** Exiting...");
		}

		internal static void Usage(int errorCode = 0)
		{
			Console.WriteLine("***********************************************************************************************");
			Console.WriteLine("***");
			Console.WriteLine("*** {0} - {1}", Name, Description);
			Console.WriteLine("***");
			Console.WriteLine("*** Usage: {0} {1}", Name, ParameterNames);
			Console.WriteLine("***");
			PrintUsageMessage();
			Console.WriteLine("***");
			Console.WriteLine("***********************************************************************************************");
			Environment.Exit(errorCode);
		}
	}

	public static int Run<TCallSite, TCommandLineParams>(this TCallSite _this, string[] args, Func<TCommandLineParams, int>? action = null) where TCallSite : class
	{
		try
		{
			return args.Run(action);
		}
		catch (CommandLineParameterException exception)
		{
			CommandLineApplicationClosure<TCommandLineParams>.Error(exception, "Validation failed", -1);
			throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
		}
	}

	public static void Run<TCallSite, TCommandLineParams>(this TCallSite _this, string[] args, Action<TCommandLineParams>? action = null) where TCallSite : class
	{
		try
		{
			args.Run(action);
		}
		catch (CommandLineParameterException exception)
		{
			CommandLineApplicationClosure<TCommandLineParams>.Error(exception, "Validation failed", -1);
		}
	}

	public static int Run<TCommandLineParams>(this string[] args, Func<TCommandLineParams, int>? action = null)
	{
		try
		{
			action = action ?? ((Func<TCommandLineParams, int>)((TCommandLineParams _) => 0));
			TCommandLineParams arg = CommandLineApplicationClosure<TCommandLineParams>.ProcessArguments(args);
			return action(arg);
		}
		catch (CommandLineParameterException exception)
		{
			CommandLineApplicationClosure<TCommandLineParams>.Error(exception, "Validation failed", -1);
			throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
		}
	}

	public static void Run<TCommandLineParams>(this string[] args, Action<TCommandLineParams>? action = null)
	{
		try
		{
			action = action ?? ((Action<TCommandLineParams>)delegate
			{
			});
			TCommandLineParams obj = CommandLineApplicationClosure<TCommandLineParams>.ProcessArguments(args);
			action(obj);
		}
		catch (CommandLineParameterException exception)
		{
			CommandLineApplicationClosure<TCommandLineParams>.Error(exception, "Validation failed", -1);
			throw new Exception("Should never reach here. Usage() changed to not call Environment.Exit?");
		}
	}
}
