using System.Reflection;
using BrightSword.SwissKnife;

namespace BrightSword.Squid.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Type"/> used by the Squid type creators.
    /// Provides helpers for generating concrete type names and locating members.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the non-generic portion of a type's name. For example, for
        /// a generic type whose concrete name is "Enumerable<Int32>" this
        /// returns "Enumerable". If the type is not generic the concrete name
        /// returned by <see cref="SwissKnife.TypeRenameExtensions.RenameToConcreteType"/>
        /// is returned unchanged.
        /// </summary>
        /// <param name="self">The type to inspect.</param>
        /// <returns>The non-generic part of the concrete class name.</returns>
        public static string GetNonGenericPartOfClassName(this Type self)
        {
            ArgumentNullException.ThrowIfNull(self);

            var typeName = self.RenameToConcreteType();
            if (!self.IsGenericType)
            {
                return typeName;
            }

            var idx = typeName.IndexOf('<');
            return idx >= 0 ? typeName[..idx] : typeName;
        }

        /// <summary>
        /// Finds a generic method with the given <paramref name="methodName"/> on the provided
        /// <paramref name="self"/> type and constructs a closed generic <see cref="MethodInfo"/>
        /// for the supplied <paramref name="typeParameters"/>.
        /// </summary>
        /// <param name="self">The type to search for the method on.</param>
        /// <param name="methodName">The name of the generic method to find.</param>
        /// <param name="typeParameters">Type parameters to apply to the generic method.</param>
        /// <returns>A constructed <see cref="MethodInfo"/> representing the closed generic method.</returned>
        public static MethodInfo GetGenericMethodOnType(this Type self, string methodName, params Type[] typeParameters)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(methodName);

            return self.GetAllMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Single(m => m.Name.Equals(methodName, StringComparison.Ordinal) && m.IsGenericMethod)
                        .MakeGenericMethod(typeParameters);
        }

        /// <summary>
        /// Returns all properties of the type excluding properties declared by the optional
        /// <paramref name="excludedTypes"/>.
        /// </summary>
        /// <param name="self">The type to inspect.</param>
        /// <param name="excludedTypes">Types whose properties should be excluded from the result.</param>
        /// <returns>An enumerable of <see cref="PropertyInfo"/> not declared on the excluded types.</returns>
        public static IEnumerable<PropertyInfo> GetAllNonExcludedProperties(this Type self, params Type[] excludedTypes)
            => self.GetAllNonExcludedMembers(_ => _.GetAllProperties(), excludedTypes);

        /// <summary>
        /// Returns all methods of the type excluding methods declared by the optional
        /// <paramref name="excludedTypes"/>.
        /// </summary>
        /// <param name="self">The type to inspect.</param>
        /// <param name="excludedTypes">Types whose methods should be excluded from the result.</param>
        /// <returns>An enumerable of <see cref="MethodInfo"/> not declared on the excluded types.</returns>
        public static IEnumerable<MethodInfo> GetAllNonExcludedMethods(this Type self, params Type[] excludedTypes)
            => self.GetAllNonExcludedMembers(_ => _.GetAllMethods(), excludedTypes);

        /// <summary>
        /// Returns all events of the type excluding events declared by the optional
        /// <paramref name="excludedTypes"/>.
        /// </summary>
        /// <param name="self">The type to inspect.</param>
        /// <param name="excludedTypes">Types whose events should be excluded from the result.</param>
        /// <returns>An enumerable of <see cref="EventInfo"/> not declared on the excluded types.</returns>
        public static IEnumerable<EventInfo> GetAllNonExcludedEvents(this Type self, params Type[] excludedTypes)
            => self.GetAllNonExcludedMembers(_ => _.GetAllEvents(), excludedTypes);

        private static IEnumerable<TMemberInfo> GetAllNonExcludedMembers<TMemberInfo>(this Type self, Func<Type, IEnumerable<TMemberInfo>> accessor, IEnumerable<Type> excludedTypes) where TMemberInfo : MemberInfo
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(accessor);

            excludedTypes ??= [];
            return accessor(self).Except(excludedTypes.SelectMany(accessor));
        }
    }
}
