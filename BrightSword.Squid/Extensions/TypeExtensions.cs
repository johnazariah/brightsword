using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BrightSword.SwissKnife;

namespace BrightSword.Squid
{
    public static class TypeExtensions
    {
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

        public static MethodInfo GetGenericMethodOnType(this Type self, string methodName, params Type[] typeParameters)
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(methodName);

            return self.GetAllMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Single(m => m.Name.Equals(methodName, StringComparison.Ordinal) && m.IsGenericMethod)
                        .MakeGenericMethod(typeParameters);
        }

        public static IEnumerable<PropertyInfo> GetAllNonExcludedProperties(this Type self, params Type[] excludedTypes)
            => self.GetAllNonExcludedMembers(_ => _.GetAllProperties(), excludedTypes);

        public static IEnumerable<MethodInfo> GetAllNonExcludedMethods(this Type self, params Type[] excludedTypes)
            => self.GetAllNonExcludedMembers(_ => _.GetAllMethods(), excludedTypes);

        public static IEnumerable<EventInfo> GetAllNonExcludedEvents(this Type self, params Type[] excludedTypes)
            => self.GetAllNonExcludedMembers(_ => _.GetAllEvents(), excludedTypes);

        private static IEnumerable<TMemberInfo> GetAllNonExcludedMembers<TMemberInfo>(this Type self,
                                                                                      Func<Type, IEnumerable<TMemberInfo>> accessor,
                                                                                      IEnumerable<Type> excludedTypes) where TMemberInfo : MemberInfo
        {
            ArgumentNullException.ThrowIfNull(self);
            ArgumentNullException.ThrowIfNull(accessor);

            excludedTypes ??= [];
            return accessor(self).Except(excludedTypes.SelectMany(accessor));
        }
    }
}
