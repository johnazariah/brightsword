using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Provides extension methods for <see cref="Type"/> to simplify reflection and property inspection.
    /// </summary>
    /// <remarks>
    /// These helpers make it easier to work with generic types, property discovery, and interface inheritance.
    /// </remarks>
    public static class TypeExtensions
    {
        private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        /// <summary>
        /// Gets a friendly name for the type, including generic arguments if applicable.
        /// </summary>
        /// <param name="this">The type to get the name for.</param>
        /// <returns>A string representing the type name, e.g. <c>List<String></c> or <c>Int32</c>.</returns>
        /// <example>
        /// <code>
        /// var name = typeof(List<string>).Name(); // "List<String>"
        /// var name2 = typeof(int).Name(); // "Int32"
        /// </code>
        /// </example>
        public static string Name(this Type @this)
        {
            if (!@this.IsGenericType)
            {
                return @this.Name;
            }

            var genericTypeDefinition = @this.GetGenericTypeDefinition();
            var baseName = genericTypeDefinition.Name[..genericTypeDefinition.Name.IndexOf('`')];
            var args = string.Join(", ", @this.GetGenericArguments().Select(a => a.Name()));
            return $"{baseName}<{args}>";
        }

        /// <summary>
        /// Gets all public instance properties for the type, including inherited properties and interface properties.
        /// </summary>
        /// <param name="this">The type to inspect.</param>
        /// <param name="bindingFlags">Binding flags to use (defaults to public instance).</param>
        /// <returns>An enumerable of <see cref="PropertyInfo"/> for all matching properties.</returns>
        /// <example>
        /// <code>
        /// var props = typeof(IMyInterface).GetAllProperties();
        /// foreach (var p in props) Console.WriteLine(p.Name);
        /// </code>
        /// </example>
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type @this, BindingFlags bindingFlags = DefaultBindingFlags)
            => @this.IsInterface ? @this.GetInterfaceProperties(bindingFlags, []) : @this.GetClassProperties(bindingFlags);

        /// <summary>
        /// Gets a property by name, optionally searching interface inheritance hierarchy.
        /// </summary>
        /// <param name="this">The type to inspect.</param>
        /// <param name="propertyName">The name of the property to find.</param>
        /// <param name="walkInterfaceInheritanceHierarchy">If true, searches all inherited interfaces for the property.</param>
        /// <returns>The <see cref="PropertyInfo"/> if found, otherwise null.</returns>
        /// <example>
        /// <code>
        /// var prop = typeof(IMyInterface).GetProperty("MyProp", true);
        /// if (prop != null) Console.WriteLine(prop.PropertyType);
        /// </code>
        /// </example>
        public static PropertyInfo GetProperty(this Type @this, string propertyName, bool walkInterfaceInheritanceHierarchy)
            => walkInterfaceInheritanceHierarchy ? @this.GetAllProperties().FirstOrDefault(_pi => _pi.Name == propertyName) : @this.GetProperty(propertyName);

        // Private helpers for property discovery
        private static IEnumerable<PropertyInfo> GetClassProperties(this Type @this, BindingFlags bindingFlags)
        {
            if (@this.IsInterface)
            {
                yield break;
            }

            bindingFlags &= ~BindingFlags.DeclaredOnly;
            foreach (var property in @this.GetProperties(bindingFlags))
            {
                yield return property;
            }
        }

        private static IEnumerable<PropertyInfo> GetInterfaceProperties(this Type @this, BindingFlags bindingFlags, IList<Type> processedInterfaces)
        {
            if (!@this.IsInterface)
            {
                yield break;
            }

            bindingFlags |= BindingFlags.DeclaredOnly;
            processedInterfaces ??= [];
            if (processedInterfaces.Contains(@this))
            {
                yield break;
            }

            foreach (var property in @this.GetProperties(bindingFlags))
            {
                yield return property;
            }

            foreach (var pi in @this.GetInterfaces().SelectMany(i => i.GetInterfaceProperties(bindingFlags, processedInterfaces)))
            {
                yield return pi;
            }

            processedInterfaces.Add(@this);
        }
    }
}
