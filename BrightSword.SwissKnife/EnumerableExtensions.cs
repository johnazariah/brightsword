using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace BrightSword.SwissKnife;

public static class EnumerableExtensions
{
  public static bool None<T>(this IEnumerable<T> _this, Func<T, bool> filter = null)
  {
    return filter == null ? !_this.Any<T>() : !_this.Any<T>(filter);
  }

  public static bool AllUnique<T>(this IEnumerable<T> _this)
  {
    return _this.All<T>(new Func<T, bool>(new HashSet<T>().Add));
  }

  public static bool AllUniqueSorted<T>(this IList<T> _this)
  {
    int index1 = -1;
    for (int index2 = 0; index2 < _this.Count; index1 = index2++)
    {
      if (index1 != -1 && _this[index1].Equals((object) _this[index2]))
        return false;
    }
    return true;
  }

  public static T LastButOne<T>(this IEnumerable<T> _this)
  {
    return _this == null ? default (T) : _this.Reverse<T>().Skip<T>(1).FirstOrDefault<T>();
  }
}
