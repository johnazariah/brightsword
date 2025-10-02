using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife;

public static class TypeExtensions
{
    private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public;

    public static string Name(this Type @this)
    {
        if (!@this.IsGenericType)
            return @this.Name;

        var genericTypeDefinition = @this.GetGenericTypeDefinition();
        var baseName = genericTypeDefinition.Name.Substring(0, genericTypeDefinition.Name.IndexOf('`'));
        var args = string.Join(", ", @this.GetGenericArguments().Select(a => a.Name()));
        return $"{baseName}<{args}>";
    }

    public static IEnumerable<PropertyInfo> GetAllProperties(this Type @this, BindingFlags bindingFlags = DefaultBindingFlags)
        => @this.IsInterface ? @this.GetInterfaceProperties(bindingFlags, new List<Type>()) : @this.GetClassProperties(bindingFlags);

    private static IEnumerable<PropertyInfo> GetClassProperties(this Type @this, BindingFlags bindingFlags)
    {
        if (@this.IsInterface)
            yield break;

        bindingFlags &= ~BindingFlags.DeclaredOnly;
        foreach (var property in @this.GetProperties(bindingFlags))
            yield return property;
    }

    private static IEnumerable<PropertyInfo> GetInterfaceProperties(this Type @this, BindingFlags bindingFlags, IList<Type> processedInterfaces)
    {
        if (!@this.IsInterface)
            yield break;

        bindingFlags |= BindingFlags.DeclaredOnly;
        processedInterfaces ??= new List<Type>();
        if (processedInterfaces.Contains(@this))
            yield break;

        foreach (var property in @this.GetProperties(bindingFlags))
            yield return property;

        foreach (var pi in @this.GetInterfaces().SelectMany(i => i.GetInterfaceProperties(bindingFlags, processedInterfaces)))
            yield return pi;

        processedInterfaces.Add(@this);
    }

    public static PropertyInfo GetProperty(this Type @this, string propertyName, bool walkInterfaceInheritanceHierarchy)
        => walkInterfaceInheritanceHierarchy ? @this.GetAllProperties().FirstOrDefault(_pi => _pi.Name == propertyName) : @this.GetProperty(propertyName);
}
