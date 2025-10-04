using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Provides extension methods for <see cref="Type"/> to simplify reflection and type-name helpers.
    /// This file adds a small set of helpers the Squid project expects: PrintableName, RenameToConcreteType,
    /// and methods to enumerate members across interface inheritance.
    /// </summary>
    public static class TypeExtensions
    {
        private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        /// <summary>
        /// Friendly printable name similar to the original project's intent.
        /// </summary>
        public static string PrintableName(this Type @this)
        {
            ArgumentNullException.ThrowIfNull(@this);

            if (!@this.IsGenericType)
            {
                return @this.Name;
            }

            var genericTypeDefinition = @this.GetGenericTypeDefinition();
            var baseName = genericTypeDefinition.Name;
            var tickIndex = baseName.IndexOf('`');
            if (tickIndex > 0) baseName = baseName.Substring(0, tickIndex);

            var args = string.Join(", ", @this.GetGenericArguments().Select(a => a.PrintableName()));
            return $"{baseName}<{args}>";
        }

        /// <summary>
        /// Backwards-compatible Name() extension used by tests and older code.
        /// </summary>
        public static string Name(this Type @this) => PrintableName(@this);

        /// <summary>
        /// Heuristic used by the Squid project to convert interface type names to a concrete class-like name.
        /// Example: IMyInterface -> MyInterface; IList&lt;T&gt; -> List&lt;T&gt;.
        /// </summary>
        public static string RenameToConcreteType(this Type @this)
        {
            ArgumentNullException.ThrowIfNull(@this);

            // If it's an interface and starts with 'I' followed by uppercase letter, trim the leading 'I'.
            var name = @this.IsGenericType ? @this.PrintableName() : @this.Name;
            if (@this.IsInterface && name.Length >= 2 && name[0] == 'I' && char.IsUpper(name[1]))
            {
                return name.Substring(1);
            }

            return name;
        }

        /// <summary>
        /// Return all properties including inherited interface properties and base class properties.
        /// </summary>
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type @this, BindingFlags bindingFlags = DefaultBindingFlags)
        {
            ArgumentNullException.ThrowIfNull(@this);

            if (@this.IsInterface)
            {
                // Include properties from all inherited interfaces
                var processed = new HashSet<Type>();
                return GetInterfaceProperties(@this, bindingFlags, processed);
            }

            bindingFlags &= ~BindingFlags.DeclaredOnly;
            return @this.GetProperties(bindingFlags);
        }

        private static IEnumerable<PropertyInfo> GetInterfaceProperties(Type @this, BindingFlags bindingFlags, HashSet<Type> processed)
        {
            bindingFlags |= BindingFlags.DeclaredOnly;
            if (!processed.Add(@this)) yield break;

            foreach (var p in @this.GetProperties(bindingFlags)) yield return p;

            foreach (var i in @this.GetInterfaces())
            {
                foreach (var p in GetInterfaceProperties(i, bindingFlags, processed)) yield return p;
            }
        }

        /// <summary>
        /// Methods and events helpers used by Squid.
        /// </summary>
        public static IEnumerable<MethodInfo> GetAllMethods(this Type @this, BindingFlags bindingFlags = DefaultBindingFlags)
        {
            ArgumentNullException.ThrowIfNull(@this);

            if (@this.IsInterface)
            {
                var processed = new HashSet<Type>();
                return GetInterfaceMethods(@this, bindingFlags, processed);
            }

            bindingFlags &= ~BindingFlags.DeclaredOnly;
            return @this.GetMethods(bindingFlags);
        }

        private static IEnumerable<MethodInfo> GetInterfaceMethods(Type @this, BindingFlags bindingFlags, HashSet<Type> processed)
        {
            bindingFlags |= BindingFlags.DeclaredOnly;
            if (!processed.Add(@this)) yield break;

            foreach (var m in @this.GetMethods(bindingFlags)) yield return m;

            foreach (var i in @this.GetInterfaces())
            {
                foreach (var m in GetInterfaceMethods(i, bindingFlags, processed)) yield return m;
            }
        }

        public static IEnumerable<EventInfo> GetAllEvents(this Type @this, BindingFlags bindingFlags = DefaultBindingFlags)
        {
            ArgumentNullException.ThrowIfNull(@this);

            if (@this.IsInterface)
            {
                var processed = new HashSet<Type>();
                return GetInterfaceEvents(@this, bindingFlags, processed);
            }

            bindingFlags &= ~BindingFlags.DeclaredOnly;
            return @this.GetEvents(bindingFlags);
        }

        private static IEnumerable<EventInfo> GetInterfaceEvents(Type @this, BindingFlags bindingFlags, HashSet<Type> processed)
        {
            bindingFlags |= BindingFlags.DeclaredOnly;
            if (!processed.Add(@this)) yield break;

            foreach (var e in @this.GetEvents(bindingFlags)) yield return e;

            foreach (var i in @this.GetInterfaces())
            {
                foreach (var e in GetInterfaceEvents(i, bindingFlags, processed)) yield return e;
            }
        }
    }
}
