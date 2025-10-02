using System;
using System.Collections.Generic;
using System.Linq;

namespace BrightSword.SwissKnife
{
    public static class EnumerableExtensions
    {
        public static bool None<T>(this IEnumerable<T> @this, Func<T, bool> filter = null)
            => filter == null ? !@this.Any() : !@this.Any(filter);

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

        public static T LastButOne<T>(this IEnumerable<T> @this)
        {
            if (@this == null)
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
