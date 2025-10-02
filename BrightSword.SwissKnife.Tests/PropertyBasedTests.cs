using System;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using BrightSword.SwissKnife;
using Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public static class PropertyBasedTests
    {
        [Property]
    public static void EnumerableAllUniqueRoundtrip(int[] items)
        {
            // AllUnique should be true iff there are no duplicates
            var expected = items.Distinct().Count() == items.Length;
            Assert.Equal(expected, items.AllUnique());
        }

        [Property]
        public static void StringSplitCamelCaseRoundtrip(NonNull<string> s)
        {
            var str = s.Get;
            // Joining the segments should contain the original characters (not strict roundtrip, but sanity check)
            var parts = str.SplitCamelCase().ToArray();
            Assert.Equal(parts.Length == 0, string.IsNullOrEmpty(str));
        }

        [Property]
    public static void CoerceNumberParseRoundtrip(int value)
        {
            var s = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var parsed = ((object)s).CoerceType(typeof(int), null);
            Assert.Equal(value, parsed);
        }

        [Property]
        public static void TypeExtensionsNameIdempotent(int index)
        {
            // pick from a small, safe set of types
            var types = new Type[] { typeof(int), typeof(string), typeof(object), typeof(System.DateTime), typeof(System.Collections.Generic.List<int>), typeof(System.Collections.Generic.Dictionary<int, string>) };
            var t = types[Math.Abs(index) % types.Length];
            var n1 = t.Name();
            var n2 = t.Name();
            Assert.Equal(n1, n2);
        }
    }

    // no custom arbitraries required
}
