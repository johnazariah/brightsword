namespace BrightSword.Squid.Extensions
{
    /// <summary>
    /// Helpers to map between generic or mappable types and their intended concrete counterparts.
    /// These extension methods are used by the runtime type generator to choose appropriate
    /// concrete backing types for interface properties (for example mapping <c>IList{T}</c> to <c>List{T}</c>).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="MapGenericTypeIfPossible(Type, Type, Type[])"/> attempts to construct the
    /// <paramref name="mapTo"/> generic type using the generic arguments from <paramref name="mapFrom"/>
    /// and then delegates to <see cref="MapTypeIfPossible(Type, Type, Type[])"/> to determine
    /// whether the mapping should be applied based on the <paramref name="mappableTypes"/>.
    /// </para>
    /// </remarks>
    public static class TypeMapExtensions
    {
        /// <summary>
        /// If <paramref name="mapFrom"/> is a generic type, attempts to construct <paramref name="mapTo"/>
        /// using the same generic type arguments and returns it when one of the <paramref name="mappableTypes"/>
        /// is assignable to <paramref name="mapFrom"/>. Returns <c>null</c> when no mapping applies.
        /// </summary>
        /// <param name="mapFrom">The source type to map from (typically a declared property type).</param>
        /// <param name="mapTo">The generic type definition to construct with the source's generic arguments.</param>
        /// <param name="mappableTypes">Candidate types whose assignability will determine whether the mapping applies.</param>
        /// <returns>The constructed target type when applicable; otherwise <c>null</c>.</returns>
        public static Type MapGenericTypeIfPossible(this Type mapFrom,
                                                    Type mapTo,
                                                    params Type[] mappableTypes)
        {
            ArgumentNullException.ThrowIfNull(mapFrom);
            ArgumentNullException.ThrowIfNull(mapTo);

            if (!mapFrom.IsGenericType)
            {
                return null;
            }

            var fromArgs = mapFrom.GetGenericArguments();
            var toArgs = mapTo.GetGenericArguments();
            return fromArgs.Length != toArgs.Length
                ? null
                : mapFrom.GetGenericTypeDefinition()
                          .MapTypeIfPossible(mapTo.MakeGenericType(fromArgs), mappableTypes);
        }

        /// <summary>
        /// Returns <paramref name="mapTo"/> when any of the <paramref name="mappableTypes"/>
        /// is assignable to <paramref name="mapFrom"/>, otherwise returns <c>null</c>.
        /// </summary>
        /// <param name="mapFrom">Source type against which assignability is tested.</param>
        /// <param name="mapTo">Target type to return when a mapping is applicable.</param>
        /// <param name="mappableTypes">Types that, if assignable to <paramref name="mapFrom"/>, cause the mapping to apply.</param>
        /// <returns><paramref name="mapTo"/> when mapping is applicable, otherwise <c>null</c>.</returns>
        /// <example>
        /// <code>
        /// // If mapFrom is IList<int> and mappableTypes contains List<>, this returns the constructed List<int>
        /// var mapped = typeof(IList<int>).MapGenericTypeIfPossible(typeof(List<>), typeof(List<>));
        /// Debug.Assert(mapped == typeof(List<int>));
        /// </code>
        /// </example>
        public static Type MapTypeIfPossible(this Type mapFrom,
                                             Type mapTo,
                                             params Type[] mappableTypes)
        {
            ArgumentNullException.ThrowIfNull(mapFrom);
            ArgumentNullException.ThrowIfNull(mapTo);

            mappableTypes ??= [];

            foreach (var mappableType in mappableTypes)
            {
                if (mapFrom.IsAssignableFrom(mappableType))
                {
                    return mapTo;
                }
            }

            return null;
        }
    }
}
