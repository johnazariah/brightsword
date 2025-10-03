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
            var typeName = self.RenameToConcreteType();

            return self.IsGenericType
                       ? typeName.Remove(typeName.IndexOf('<'))
                       : typeName;
        }

        public static MethodInfo GetGenericMethodOnType(this Type self,
                                                        string methodName,
                                                        params Type[] typeParameters)
        {
            return self.GetAllMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Single(_ => _.Name.Equals(methodName, StringComparison.Ordinal) && _.IsGenericMethod)
                        .MakeGenericMethod(typeParameters);
        }

        public static IEnumerable<PropertyInfo> GetAllNonExcludedProperties(this Type self,
                                                                            params Type[] excludedTypes)
        {
            return self.GetAllNonExcludedMembers(_ => _.GetAllProperties(),
                                                  excludedTypes);
        }

        public static IEnumerable<MethodInfo> GetAllNonExcludedMethods(this Type self,
                                                                       params Type[] excludedTypes)
        {
            return self.GetAllNonExcludedMembers(_ => _.GetAllMethods(),
                                                  excludedTypes);
        }

        public static IEnumerable<EventInfo> GetAllNonExcludedEvents(this Type self,
                                                                     params Type[] excludedTypes)
        {
            return self.GetAllNonExcludedMembers(_ => _.GetAllEvents(),
                                                  excludedTypes);
        }

        private static IEnumerable<TMemberInfo> GetAllNonExcludedMembers<TMemberInfo>(this Type self,
                                                                                      Func<Type, IEnumerable<TMemberInfo>> accessor,
                                                                                      IEnumerable<Type> excludedTypes) where TMemberInfo : MemberInfo
        {
            return accessor(self)
                .Except(excludedTypes.SelectMany(accessor));
        }
    }
}
