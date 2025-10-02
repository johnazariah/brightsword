using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace BrightSword.SwissKnife;

public static class StringExtensions
{
  public static IEnumerable<string> SplitCamelCase(this string _this)
  {
    return _this.SplitIntoSegments(splitOnPunctuation: false);
  }

  public static IEnumerable<string> SplitCamelCaseAndUnderscore(this string _this)
  {
    return _this.SplitIntoSegments(true, true, true, '_');
  }

  public static IEnumerable<string> SplitDotted(this string _this)
  {
    return _this.SplitIntoSegments(false, false, true, '.');
  }

  public static IEnumerable<string> SplitIntoSegments(
    this string _this,
    bool splitBySpace = true,
    bool splitOnCamelCase = true,
    bool splitOnPunctuation = true,
    params char[] separators)
  {
    if (!string.IsNullOrEmpty(_this))
    {
      int iStart = 0;
      int iEnd = 1;
      while (true)
      {
        bool endOfString;
        string segment = _this.GetNextSegment(ref iStart, ref iEnd, out endOfString, splitBySpace, splitOnCamelCase, splitOnPunctuation, separators);
        if (!string.IsNullOrWhiteSpace(segment))
          yield return segment;
        if (!endOfString)
          iStart = iEnd++;
        else
          break;
      }
    }
  }

  private static string GetNextSegment(
    this string _this,
    ref int iStart,
    ref int iEnd,
    out bool endOfString,
    bool respectSpace = true,
    bool respectCamelCase = true,
    bool respectPunctuation = true,
    params char[] separators)
  {
    endOfString = iEnd == _this.Length;
    if (respectSpace)
    {
      while (char.IsWhiteSpace(_this[iStart]))
      {
        iEnd = ++iStart;
        if (iStart == _this.Length)
        {
          endOfString = true;
          break;
        }
      }
    }
    if (respectPunctuation && _this[iStart].IsRecognizedPunctuationMark(separators))
    {
      ++iStart;
      if (iStart == _this.Length)
      {
        endOfString = true;
        goto label_14;
      }
      iEnd = iStart;
    }
label_14:
    while (!endOfString && (!respectSpace || !char.IsWhiteSpace(_this[iEnd])) && (!respectPunctuation || !_this[iEnd].IsRecognizedPunctuationMark(separators)))
    {
      if (respectCamelCase && char.IsUpper(_this[iEnd]))
      {
        if (char.IsUpper(_this[iEnd - 1]))
        {
          int index = iEnd + 1;
          if (index < _this.Length && !char.IsUpper(_this[index]) && !char.IsWhiteSpace(_this[index]) && !_this[index].IsRecognizedPunctuationMark(separators))
            break;
        }
        else
          break;
      }
      ++iEnd;
      endOfString = iEnd == _this.Length;
    }
    return _this.Substring(iStart, iEnd - iStart);
  }

  private static bool IsRecognizedPunctuationMark(this char _this, char[] separators)
  {
    return separators.Length == 0 ? char.IsPunctuation(_this) : ((IEnumerable<char>) separators).Contains<char>(_this);
  }
}
