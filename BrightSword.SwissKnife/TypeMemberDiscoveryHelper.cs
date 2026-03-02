#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife;

internal static class TypeMemberDiscoveryHelper
{
	internal static IEnumerable<TMember> GetAllMembers<TMember>(this Type type, Func<Type, BindingFlags, IEnumerable<TMember>> accessor, BindingFlags bindingFlags) where TMember : MemberInfo
	{
		return type.IsInterface ? type.GetInterfaceMembers((Type _) => accessor(_, bindingFlags | BindingFlags.DeclaredOnly)) : (type.IsClass ? type.GetClassMembers((Type _) => accessor(_, bindingFlags & ~BindingFlags.DeclaredOnly)) : type.GetStructMembers((Type _) => accessor(_, bindingFlags & ~BindingFlags.DeclaredOnly)));
	}

	private static IEnumerable<TMember> GetClassMembers<TMember>(this Type type, Func<Type, IEnumerable<TMember>> accessor) where TMember : MemberInfo
	{
		Debug.Assert(type.IsClass);
		return accessor(type);
	}

	private static IEnumerable<TMember> GetStructMembers<TMember>(this Type type, Func<Type, IEnumerable<TMember>> accessor) where TMember : MemberInfo
	{
		Debug.Assert(type.IsValueType);
		return accessor(type);
	}

	private static IEnumerable<TMember> GetInterfaceMembers<TMember>(this Type type, Func<Type, IEnumerable<TMember>> accessor, ISet<Type>? processedInterfaces = null) where TMember : MemberInfo
	{
		Debug.Assert(type.IsInterface);
		processedInterfaces = processedInterfaces ?? new HashSet<Type>();
		if (processedInterfaces.Contains(type))
		{
			yield break;
		}
		foreach (TMember item in accessor(type))
		{
			yield return item;
		}
		foreach (TMember item2 in type.GetInterfaces().SelectMany((Type _) => _.GetInterfaceMembers(accessor, processedInterfaces)))
		{
			yield return item2;
		}
		processedInterfaces.Add(type);
	}
}
