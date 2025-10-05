using System.Reflection;

namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Provides extension methods for retrieving custom attributes and their values from types and members.
    /// </summary>
    /// <remarks>
    /// These helpers simplify attribute discovery and value extraction for reflection scenarios.
    /// </remarks>
    public static class AttributeExtensions
    {
        /// <summary>
        /// Gets the first custom attribute of the specified type applied to a <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type to search for.</typeparam>
        /// <param name="this">The type to inspect.</param>
        /// <param name="flags">Binding flags (not used).</param>
        /// <returns>The first matching attribute, or null if not found.</returns>
        /// <example>
        /// <code>
        /// [Serializable]
        /// public class MyClass { }
        /// var attr = typeof(MyClass).GetCustomAttribute<SerializableAttribute>(); // not null
        /// </code>
        /// </example>
        public static TAttribute GetCustomAttribute<TAttribute>(this Type @this, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute
        {
            _ = flags;
            return @this.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the first custom attribute of the specified type applied to a <see cref="MemberInfo"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type to search for.</typeparam>
        /// <param name="this">The member to inspect.</param>
        /// <param name="flags">Binding flags (not used).</param>
        /// <returns>The first matching attribute, or null if not found.</returns>
        /// <example>
        /// <code>
        /// public class MyClass {
        ///     [Obsolete]
        ///     public void OldMethod() { }
        /// }
        /// var method = typeof(MyClass).GetMethod("OldMethod");
        /// var attr = method.GetCustomAttribute<ObsoleteAttribute>(); // not null
        /// </code>
        /// </example>
        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo @this, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute
        {
            _ = flags;
            return @this.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// Gets a value from the first custom attribute of the specified type applied to a <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type to search for.</typeparam>
        /// <typeparam name="TResult">The result type to extract.</typeparam>
        /// <param name="this">The type to inspect.</param>
        /// <param name="selector">A function to extract the value from the attribute.</param>
        /// <param name="defaultValue">The value to return if the attribute is not found.</param>
        /// <param name="flags">Binding flags (not used).</param>
        /// <returns>The extracted value, or <paramref name="defaultValue"/> if not found.</returns>
        /// <example>
        /// <code>
        /// [Serializable]
        /// public class MyClass { }
        /// var isSerializable = typeof(MyClass).GetCustomAttributeValue<SerializableAttribute, bool>(attr => true, false);
        /// </code>
        /// </example>
        public static TResult GetCustomAttributeValue<TAttribute, TResult>(this Type @this, Func<TAttribute, TResult> selector, TResult defaultValue = default, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TAttribute : Attribute
        {
            _ = flags;
            var attr = @this.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
            return attr is null ? defaultValue : selector(attr);
        }

        /// <summary>
        /// Gets a value from the first custom attribute of the specified type applied to a <see cref="MemberInfo"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type to search for.</typeparam>
        /// <typeparam name="TResult">The result type to extract.</typeparam>
        /// <param name="this">The member to inspect.</param>
        /// <param name="selector">A function to extract the value from the attribute.</param>
        /// <param name="defaultValue">The value to return if the attribute is not found.</param>
        /// <param name="flags">Binding flags (not used).</param>
        /// <returns>The extracted value, or <paramref name="defaultValue"/> if not found.</returns>
        /// <example>
        /// <code>
        /// public class MyClass {
        ///     [Obsolete("Use NewMethod")]
        ///     public void OldMethod() { }
        /// }
        /// var method = typeof(MyClass).GetMethod("OldMethod");
        /// var message = method.GetCustomAttributeValue<ObsoleteAttribute, string>(attr => attr.Message, "");
        /// </code>
        /// </example>
        public static TResult GetCustomAttributeValue<TAttribute, TResult>(this MemberInfo @this, Func<TAttribute, TResult> selector, TResult defaultValue = default, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TAttribute : Attribute
        {
            _ = flags;
            var attr = @this.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
            return attr is null ? defaultValue : selector(attr);
        }
    }
}
