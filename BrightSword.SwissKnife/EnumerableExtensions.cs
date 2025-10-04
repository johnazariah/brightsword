using System;
using System.Collections.Generic;
using System.Linq;

namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Provides extension methods for <see cref="IEnumerable{T}"/> and <see cref="IList{T}"/> for common collection operations.
    /// </summary>
    /// <remarks>
    /// These helpers simplify checks for uniqueness, emptiness, and access to elements.
    /// </remarks>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns true if the sequence contains no elements, or no elements matching the filter.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="this">The sequence to check.</param>
        /// <param name="filter">Optional filter predicate.</param>
        /// <returns>True if no elements (or no elements matching the filter) are present.</returns>
        /// <example>
        /// <code>
        /// var empty = new int[0].None(); // true
        /// var noneEven = new[] { 1, 3, 5 }.None(x => x % 2 == 0); // true
        /// </code>
        /// </example>
        [Obsolete("Use .Any() or .All() with a predicate instead. None<T> is superseded by built-in LINQ methods.")]
        public static bool None<T>(this IEnumerable<T> @this, Func<T, bool> filter = null)
            => filter is null ? !@this.Any() : !@this.Any(filter);

        /// <summary>
        /// Returns true if all elements in the sequence are unique.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="this">The sequence to check.</param>
        /// <returns>True if all elements are unique, false if any duplicates are found.</returns>
        /// <example>
        /// <code>
        /// var unique = new[] { 1, 2, 3 }.AllUnique(); // true
        /// var notUnique = new[] { 1, 2, 2 }.AllUnique(); // false
        /// </code>
        /// </example>
        public static bool AllUnique<T>(this IEnumerable<T> @this)
        {
            if (@this is null)
            {
                return true;
            }

            var seen = new HashSet<T>();
            foreach (var item in @this)
            {
                if (!seen.Add(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if all elements in the sorted list are unique (no adjacent duplicates).
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="this">The sorted list to check.</param>
        /// <returns>True if all elements are unique, false if any adjacent duplicates are found.</returns>
        /// <example>
        /// <code>
        /// var sortedUnique = new List<int> { 1, 2, 3 }.AllUniqueSorted(); // true
        /// var sortedNotUnique = new List<int> { 1, 2, 2, 3 }.AllUniqueSorted(); // false
        /// </code>
        /// </example>
        public static bool AllUniqueSorted<T>(this IList<T> @this)
        {
            for (var i = 1; i < @this.Count; ++i)
            {
                if (@this[i - 1].Equals(@this[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the second-to-last element in the sequence, or default if not available.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="this">The sequence to check.</param>
        /// <returns>The second-to-last element, or default if the sequence has fewer than two elements.</returns>
        /// <example>
        /// <code>
        /// var arr = new[] { 1, 2, 3 };
        /// var lastButOne = arr.LastButOne(); // 2
        /// var single = new[] { 42 }.LastButOne(); // 0 (default)
        /// </code>
        /// </example>
        public static T LastButOne<T>(this IEnumerable<T> @this)
        {
            if (@this is null)
            {
                return default;
            }

            if (@this is IList<T> list)
            {
                return list.Count < 2 ? default : list[list.Count - 2];
            }

            T prev = default;
            T last = default;
            var hasPrev = false;
            foreach (var item in @this)
            {
                prev = last;
                last = item;
                hasPrev = true;
            }

            return hasPrev ? prev : default;
        }
    }
}
