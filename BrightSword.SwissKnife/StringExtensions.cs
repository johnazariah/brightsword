using System.Collections.Generic;
using System.Linq;

namespace BrightSword.SwissKnife;

public static class StringExtensions
{
    public static IEnumerable<string> SplitCamelCase(this string @this) => @this.SplitIntoSegments(splitOnPunctuation: false);

    public static IEnumerable<string> SplitCamelCaseAndUnderscore(this string @this) => @this.SplitIntoSegments(true, true, true, '_');

    public static IEnumerable<string> SplitDotted(this string @this) => @this.SplitIntoSegments(false, false, true, '.');

    public static IEnumerable<string> SplitIntoSegments(this string @this, bool splitBySpace = true, bool splitOnCamelCase = true, bool splitOnPunctuation = true, params char[] separators)
    {
        if (string.IsNullOrEmpty(@this))
            yield break;

        var iStart = 0;
        var iEnd = 1;
        while (true)
        {
            var segment = @this.GetNextSegment(ref iStart, ref iEnd, out var endOfString, splitBySpace, splitOnCamelCase, splitOnPunctuation, separators);
            if (!string.IsNullOrWhiteSpace(segment))
                yield return segment;

            if (!endOfString)
                iStart = iEnd++;
            else
                break;
        }
    }

    private static string GetNextSegment(this string @this, ref int iStart, ref int iEnd, out bool endOfString, bool respectSpace = true, bool respectCamelCase = true, bool respectPunctuation = true, params char[] separators)
    {
        endOfString = iEnd == @this.Length;
        if (respectSpace)
        {
            while (char.IsWhiteSpace(@this[iStart]))
            {
                iEnd = ++iStart;
                if (iStart == @this.Length)
                {
                    endOfString = true;
                    break;
                }
            }
        }

        if (respectPunctuation && @this[iStart].IsRecognizedPunctuationMark(separators))
        {
            ++iStart;
            if (iStart == @this.Length)
            {
                endOfString = true;
                goto label_14;
            }
            iEnd = iStart;
        }

    label_14:
        while (!endOfString && (!respectSpace || !char.IsWhiteSpace(@this[iEnd])) && (!respectPunctuation || !@this[iEnd].IsRecognizedPunctuationMark(separators)))
        {
            if (respectCamelCase && char.IsUpper(@this[iEnd]))
            {
                if (char.IsUpper(@this[iEnd - 1]))
                {
                    var index = iEnd + 1;
                    if (index < @this.Length && !char.IsUpper(@this[index]) && !char.IsWhiteSpace(@this[index]) && !@this[index].IsRecognizedPunctuationMark(separators))
                        break;
                }
                else
                {
                    break;
                }
            }

            ++iEnd;
            endOfString = iEnd == @this.Length;
        }

        return @this.Substring(iStart, iEnd - iStart);
    }

    private static bool IsRecognizedPunctuationMark(this char @this, char[] separators)
        => separators.Length == 0 ? char.IsPunctuation(@this) : separators.Contains(@this);
}
