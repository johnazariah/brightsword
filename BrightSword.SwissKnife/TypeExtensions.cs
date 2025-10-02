using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife;

public static class TypeExtensions
{
    private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public;

    public static string Name(this Type _this)
    {
        if (!_this.IsGenericType)
            return _this.Name;
        Type genericTypeDefinition = _this.GetGenericTypeDefinition();
        return $"{genericTypeDefinition.Name.Substring(0, genericTypeDefinition.Name.IndexOf('`'))}<{((IEnumerable<Type>)_this.GetGenericArguments()).Aggregate<Type, string>(string.Empty, (Func<string, Type, string>)((_res, _curr) => string.IsNullOrEmpty(_res) ? _curr.Name() : $"{_res}, {_curr.Name()}"))}>";
    }

    public static IEnumerable<PropertyInfo> GetAllProperties(
      this Type _this,
      BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
    {
        return _this.IsInterface ? _this.GetInterfaceProperties(bindingFlags, (IList<Type>)new List<Type>()) : _this.GetClassProperties(bindingFlags);
    }

    private static IEnumerable<PropertyInfo> GetClassProperties(
      this Type _this,
      BindingFlags bindingFlags)
    {
        if (!_this.IsInterface)
        {
            bindingFlags &= ~BindingFlags.DeclaredOnly;
            foreach (PropertyInfo property in _this.GetProperties(bindingFlags))
                yield return property;
        }
    }

    private static IEnumerable<PropertyInfo> GetInterfaceProperties(
      this Type _this,
      BindingFlags bindingFlags,
      IList<Type> processedInterfaces)
    {
        if (_this.IsInterface)
        {
            bindingFlags |= BindingFlags.DeclaredOnly;
            processedInterfaces = processedInterfaces ?? (IList<Type>)new List<Type>();
            if (!processedInterfaces.Contains(_this))
            {
                foreach (PropertyInfo property in _this.GetProperties(bindingFlags))
                    yield return property;
                foreach (PropertyInfo _pi in ((IEnumerable<Type>)_this.GetInterfaces()).SelectMany<Type, PropertyInfo>((Func<Type, IEnumerable<PropertyInfo>>)(_ => _.GetInterfaceProperties(bindingFlags, processedInterfaces))))
                    yield return _pi;
                processedInterfaces.Add(_this);
            }
        }
    }

    public static PropertyInfo GetProperty(
      this Type _this,
      string propertyName,
      bool walkInterfaceInheritanceHierarchy)
    {
        return walkInterfaceInheritanceHierarchy ? _this.GetAllProperties().FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>)(_pi => _pi.Name == propertyName)) : _this.GetProperty(propertyName);
    }
}
