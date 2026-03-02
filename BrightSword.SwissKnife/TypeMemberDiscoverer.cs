using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife;

public static class TypeMemberDiscoverer
{
	private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public;

	public static IEnumerable<PropertyInfo> GetAllProperties(this Type _this, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
	{
		ArgumentNullException.ThrowIfNull(_this);
		return _this.GetAllMembers((Type _type, BindingFlags _bindingFlags) => _type.GetProperties(_bindingFlags), bindingFlags);
	}

	public static IEnumerable<MethodInfo> GetAllMethods(this Type _this, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
	{
		ArgumentNullException.ThrowIfNull(_this);
		return _this.GetAllMembers((Type _type, BindingFlags _bindingFlags) => _type.GetMethods(_bindingFlags), bindingFlags);
	}

	public static IEnumerable<EventInfo> GetAllEvents(this Type _this, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
	{
		ArgumentNullException.ThrowIfNull(_this);
		return _this.GetAllMembers((Type _type, BindingFlags _bindingFlags) => _type.GetEvents(_bindingFlags), bindingFlags);
	}

	public static PropertyInfo? GetProperty(this Type _this, string propertyName, bool walkInterfaceInheritanceHierarchy)
	{
		return walkInterfaceInheritanceHierarchy ? _this.GetAllProperties().FirstOrDefault((PropertyInfo _pi) => _pi.Name == propertyName) : _this.GetProperty(propertyName);
	}
}

public static class TypeMemberDiscoverer<T>
{
	private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public;

	private static readonly Lazy<IEnumerable<PropertyInfo>> _propertiesL = new Lazy<IEnumerable<PropertyInfo>>(() => typeof(T).GetAllMembers((Type _type, BindingFlags _bindingFlags) => _type.GetProperties(_bindingFlags), BindingFlags.Instance | BindingFlags.Public));

	private static readonly Lazy<IEnumerable<EventInfo>> _eventsL = new Lazy<IEnumerable<EventInfo>>(() => typeof(T).GetAllMembers((Type _type, BindingFlags _bindingFlags) => _type.GetEvents(_bindingFlags), BindingFlags.Instance | BindingFlags.Public));

	private static readonly Lazy<IEnumerable<MethodInfo>> _methodsL = new Lazy<IEnumerable<MethodInfo>>(() => typeof(T).GetAllMembers((Type _type, BindingFlags _bindingFlags) => _type.GetMethods(_bindingFlags), BindingFlags.Instance | BindingFlags.Public));

	public static IEnumerable<PropertyInfo> GetAllProperties()
	{
		return _propertiesL.Value;
	}

	public static IEnumerable<EventInfo> GetAllEvents()
	{
		return _eventsL.Value;
	}

	public static IEnumerable<MethodInfo> GetAllMethods()
	{
		return _methodsL.Value;
	}
}
