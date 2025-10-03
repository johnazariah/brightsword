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
            ArgumentNullException.ThrowIfNull(mapFrom);
            ArgumentNullException.ThrowIfNull(mapTo);

            if (!mapFrom.IsGenericType) return null;

            var fromArgs = mapFrom.GetGenericArguments();
            var toArgs = mapTo.GetGenericArguments();
            if (fromArgs.Length != toArgs.Length) return null;

            return mapFrom.GetGenericTypeDefinition()
                          .MapTypeIfPossible(mapTo.MakeGenericType(fromArgs), mappableTypes);
        }

        public static Type MapTypeIfPossible(this Type mapFrom,
                                             Type mapTo,
                                             params Type[] mappableTypes)
        {
            ArgumentNullException.ThrowIfNull(mapFrom);
            ArgumentNullException.ThrowIfNull(mapTo);

            mappableTypes ??= [];

            foreach (var mappableType in mappableTypes)
            {
                if (mapFrom.IsAssignableFrom(mappableType)) return mapTo;
            }

            return null;
        }
    }
}
