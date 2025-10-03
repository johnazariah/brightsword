using System;
using System.Linq;

namespace BrightSword.Squid
{
    public static class TypeMapExtensions
    {
        public static Type MapGenericTypeIfPossible(this Type mapFrom,
                                                    Type mapTo,
                                                    params Type[] mappableTypes)
        {
            if (!mapFrom.IsGenericType)
            {
                return null;
            }

            if (mapFrom.GetGenericArguments().Length != mapTo.GetGenericArguments().Length)
            {
                return null;
            }

            return mapFrom.GetGenericTypeDefinition()
                          .MapTypeIfPossible(mapTo.MakeGenericType(mapFrom.GetGenericArguments()),
                                             mappableTypes);
        }

        public static Type MapTypeIfPossible(this Type mapFrom,
                                             Type mapTo,
                                             params Type[] mappableTypes)
        {
            var assignable = from mappableType in mappableTypes
                             let mapIsPossible = mapFrom.IsAssignableFrom(mappableType)
                             select mapIsPossible
                                        ? mapTo
                                        : null;

            return assignable.FirstOrDefault(_ => _ != null);
        }
    }
}