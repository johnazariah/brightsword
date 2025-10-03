using System.Collections.Generic;
using System.Linq;

namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Provides extension methods for <see cref="string"/> to split into segments by camel case, underscores, spaces, or punctuation.
    /// </summary>
    /// <remarks>
    /// These helpers simplify string tokenization for display, parsing, or formatting scenarios.
    /// </remarks>
    public static class StringExtensions
    {
        /// <summary>
        /// Splits a camel-case string into its constituent segments.
        /// </summary>
        /// <param name="this">The string to split.</param>
        /// <returns>An enumerable of segments split by camel case.</returns>
        /// <example>
        /// <code>
        /// var segments = "CamelCaseString".SplitCamelCase(); // ["Camel", "Case", "String"]
        /// </code>
        /// </example>
        public static IEnumerable<string> SplitCamelCase(this string @this) => @this.SplitIntoSegments(splitOnPunctuation: false);

        /// <summary>
        /// Splits a string into segments by camel case and underscores.
        /// </summary>
        /// <param name="this">The string to split.</param>
        /// <returns>An enumerable of segments split by camel case and underscores.</returns>
        /// <example>
        /// <code>
        /// var segments = "Camel_CaseString".SplitCamelCaseAndUnderscore(); // ["Camel", "Case", "String"]
        /// </code>
        /// </example>
        public static IEnumerable<string> SplitCamelCaseAndUnderscore(this string @this) => @this.SplitIntoSegments(true, true, true, '_');

        /// <summary>
        /// Splits a string into segments by dots.
        /// </summary>
        /// <param name="this">The string to split.</param>
        /// <returns>An enumerable of segments split by dots.</returns>
        /// <example>
        /// <code>
        /// var segments = "A.B.C".SplitDotted(); // ["A", "B", "C"]
        /// </code>
        /// </example>
        public static IEnumerable<string> SplitDotted(this string @this) => @this.SplitIntoSegments(false, false, true, '.');

        /// <summary>
        /// Splits a string into segments by spaces, camel case, punctuation, or custom separators.
        /// </summary>
        /// <param name="this">The string to split.</param>
        /// <param name="splitBySpace">Whether to split on spaces.</param>
        /// <param name="splitOnCamelCase">Whether to split on camel case.</param>
        /// <param name="splitOnPunctuation">Whether to split on punctuation.</param>
        /// <param name="separators">Custom separator characters.</param>
        /// <returns>An enumerable of segments.</returns>
        /// <example>
        /// <code>
        /// var segments = "A_B C.D".SplitIntoSegments(true, true, true, '_', '.'); // ["A", "B", "C", "D"]
        /// </code>
        /// </example>
        public static IEnumerable<string> SplitIntoSegments(this string @this, bool splitBySpace = true, bool splitOnCamelCase = true, bool splitOnPunctuation = true, params char[] separators)
        {
            if (string.IsNullOrEmpty(@this))
            {
                yield break;
            }

            var iStart = 0;
            var iEnd = 1;
            while (true)
            {
                var segment = @this.GetNextSegment(ref iStart, ref iEnd, out var endOfString, splitBySpace, splitOnCamelCase, splitOnPunctuation, separators);
                if (!string.IsNullOrWhiteSpace(segment))
                {
                    yield return segment;
                }

                if (!endOfString)
                {
                    iStart = iEnd++;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Private helper for segment extraction. Returns the next segment in the string based on the provided rules.
        /// </summary>
        /// <param name="this">The input string.</param>
        /// <param name="iStart">Start index (by reference).</param>
        /// <param name="iEnd">End index (by reference).</param>
        /// <param name="endOfString">Set to true when the end of the string is reached.</param>
        /// <param name="respectSpace">Whether to respect spaces as separators.</param>
        /// <param name="respectCamelCase">Whether to split on camel case.</param>
        /// <param name="respectPunctuation">Whether to split on punctuation.</param>
        /// <param name="separators">Custom separator characters.</param>
        /// <returns>The next segment as a string.</returns>
        private static string GetNextSegment(this string @this, ref int iStart, ref int iEnd, out bool endOfString, bool respectSpace = true, bool respectCamelCase = true, bool respectPunctuation = true, params char[] separators)
        {
            // Ensure indices are within bounds
            if (iStart >= @this.Length)
            {
                endOfString = true;
                return string.Empty;
            }

            endOfString = iEnd >= @this.Length;

            if (respectSpace)
            {
                while (iStart < @this.Length && char.IsWhiteSpace(@this[iStart]))
                {
                    iEnd = ++iStart;
                    if (iStart >= @this.Length)
                    {
                        endOfString = true;
                        break;
                    }
                }
            }

            if (respectPunctuation && iStart < @this.Length && @this[iStart].IsRecognizedPunctuationMark(separators))
            {
                ++iStart;
                if (iStart >= @this.Length)
                {
                    endOfString = true;
                    goto label_14;
                }
                iEnd = iStart;
            }

        label_14:
            while (!endOfString && iEnd < @this.Length && (!respectSpace || !char.IsWhiteSpace(@this[iEnd])) && (!respectPunctuation || !@this[iEnd].IsRecognizedPunctuationMark(separators)))
            {
                if (respectCamelCase && iEnd < @this.Length && char.IsUpper(@this[iEnd]))
                {
                    if (iEnd - 1 >= 0 && char.IsUpper(@this[iEnd - 1]))
                    {
                        var index = iEnd + 1;
                        if (index < @this.Length && !char.IsUpper(@this[index]) && !char.IsWhiteSpace(@this[index]) && !@this[index].IsRecognizedPunctuationMark(separators))
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                ++iEnd;
                endOfString = iEnd >= @this.Length;
            }

            return @this[iStart..iEnd];
        }

        private static bool IsRecognizedPunctuationMark(this char @this, char[] separators)
            => separators.Length == 0 ? char.IsPunctuation(@this) : separators.Contains(@this);
    }
}
