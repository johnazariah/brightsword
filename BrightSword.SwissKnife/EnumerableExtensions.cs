using System;
using System.Collections.Generic;
using System.Linq;

namespace BrightSword.SwissKnife;

public static class EnumerableExtensions
{
	public static bool AllUnique<T>(this IEnumerable<T> _this)
	{
		return _this.All(new HashSet<T>().Add);
	}

	public static bool SortedListIsUnique<T>(this IList<T> _this)
	{
		int num = -1;
		int num2 = 0;
		while (num2 < _this.Count)
		{
			if (num != -1 && _this[num]!.Equals(_this[num2]))
			{
				return false;
			}
			num = num2++;
		}
		return true;
	}

	public static T? LastButOne<T>(this IEnumerable<T> _this)
	{
		if (_this == null)
		{
			return default(T);
		}
		return _this.Reverse().Skip(1).FirstOrDefault();
	}

	public static bool SequenceEqual<T>(this IEnumerable<T> _this, IList<T> other) where T : IEquatable<T>
	{
		return _this.Select((T _item, int _index) => _item.Equals(other[_index])).All((bool _) => _);
	}

	public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> _this, int batchSize = 100)
	{
		List<T> currentBatch = new List<T>();
		foreach (T item in _this)
		{
			if (currentBatch.Count < batchSize)
			{
				currentBatch.Add(item);
				continue;
			}
			yield return currentBatch;
			currentBatch = new List<T> { item };
		}
		yield return currentBatch;
	}
}
