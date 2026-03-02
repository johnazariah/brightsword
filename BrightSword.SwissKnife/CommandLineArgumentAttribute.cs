using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BrightSword.SwissKnife;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class CommandLineArgumentAttribute : Attribute
{
	private const BindingFlags C_BINDING_FLAGS = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	private bool _flag;

	public bool IsOptional { get; set; }

	public bool IsFlag
	{
		get
		{
			return _flag;
		}
		set
		{
			_flag = value;
			if (_flag)
			{
				IsOptional = true;
			}
		}
	}

	internal string Name { get; set; }

	internal string Description { get; set; }

	internal object? DefaultValue { get; set; }

	internal string ArgumentDescriptor => string.Format(IsOptional ? "[{0}]" : "{0}", string.Format("--{0}{1}", Name, IsFlag ? "" : "=<value>"));

	internal string? PropertyName { get; set; }

	internal Type? ParamType { get; set; }

	internal CommandLineArgumentAttribute(string name, string description, bool flag)
	{
		Name = name;
		Description = description;
		IsFlag = flag;
	}

	public CommandLineArgumentAttribute(string name, string description)
	{
		Name = name;
		Description = description;
	}

	public CommandLineArgumentAttribute(string name, string description, object? defaultValue)
		: this(name, description)
	{
		DefaultValue = defaultValue;
		IsOptional |= defaultValue != null;
	}

	internal object? GetValue<TCommandLineParams>(TCommandLineParams _params)
	{
		string? propertyName = PropertyName;
		if (string.IsNullOrEmpty(propertyName))
		{
			return null;
		}
		PropertyInfo? property = typeof(TCommandLineParams).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null)
		{
			return property.GetValue(_params, null);
		}
		FieldInfo? field = typeof(TCommandLineParams).GetField(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			return field.GetValue(_params);
		}
		return null;
	}

	internal void SetValue<TCommandLineParams>(TCommandLineParams _params, object? value, bool fSettingDefault = false)
	{
		string? propertyName = PropertyName;
		if (string.IsNullOrEmpty(propertyName))
		{
			return;
		}
		if (fSettingDefault)
		{
			value = value ?? DefaultValue;
		}
		else if (IsFlag)
		{
			value = true;
		}
		if (IsFlag && !fSettingDefault)
		{
			value = true;
		}
		PropertyInfo? property = typeof(TCommandLineParams).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null)
		{
			object? value2 = value.CoerceType(property.PropertyType, null);
			property.SetValue(_params, value2, null);
			return;
		}
		FieldInfo? field = typeof(TCommandLineParams).GetField(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			object? value2 = value.CoerceType(field.FieldType, null);
			field.SetValue(_params, value2);
		}
	}
}
