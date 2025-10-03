using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BrightSword.SwissKnife;

namespace BrightSword.Squid
{
    public static class TypeExtensions
    {
        public static string GetNonGenericPartOfClassName(this Type _this)
        {
            var typeName = _this.RenameToConcreteType();

            return _this.IsGenericType
                       ? typeName.Remove(typeName.IndexOf('<'))
                       : typeName;
        }

        public static MethodInfo GetGenericMethodOnType(this Type _this,
                                                        string methodName,
                                                        params Type[] typeParameters)
        {
            return _this.GetAllMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Single(_ => _.Name.Equals(methodName) && _.IsGenericMethod)
                        .MakeGenericMethod(typeParameters);
        }

        public static IEnumerable<PropertyInfo> GetAllNonExcludedProperties(this Type _this,
                                                                            params Type[] excludedTypes)
        {
            return _this.GetAllNonExcludedMembers(_ => _.GetAllProperties(),
                                                  excludedTypes);
        }

        public static IEnumerable<MethodInfo> GetAllNonExcludedMethods(this Type _this,
                                                                       params Type[] excludedTypes)
        {
            return _this.GetAllNonExcludedMembers(_ => _.GetAllMethods(),
                                                  excludedTypes);
        }

        public static IEnumerable<EventInfo> GetAllNonExcludedEvents(this Type _this,
                                                                     params Type[] excludedTypes)
        {
            return _this.GetAllNonExcludedMembers(_ => _.GetAllEvents(),
                                                  excludedTypes);
        }

        private static IEnumerable<TMemberInfo> GetAllNonExcludedMembers<TMemberInfo>(this Type _this,
                                                                                      Func<Type, IEnumerable<TMemberInfo>> accessor,
                                                                                      IEnumerable<Type> excludedTypes) where TMemberInfo : MemberInfo
        {
            return accessor(_this)
                .Except(excludedTypes.SelectMany(accessor));
        }
    }
}