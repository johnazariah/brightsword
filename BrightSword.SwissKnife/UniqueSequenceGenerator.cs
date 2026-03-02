using System;
using System.Collections.Generic;
using System.Threading;

namespace BrightSword.SwissKnife;

public static class UniqueSequenceGenerator
{
	private static long _forwardCounter;

	private static long _reverseCounter;

	private static readonly SemaphoreSlim _lock;

	public static long NextDescendingUniqueValue
	{
		get
		{
			try
			{
				_lock.Wait();
				Interlocked.Decrement(ref _reverseCounter);
				return _reverseCounter;
			}
			finally
			{
				_lock.Release();
			}
		}
	}

	public static long NextAscendingUniqueValue
	{
		get
		{
			try
			{
				_lock.Wait();
				Interlocked.Increment(ref _forwardCounter);
				return _forwardCounter;
			}
			finally
			{
				_lock.Release();
			}
		}
	}

	static UniqueSequenceGenerator()
	{
		_forwardCounter = DateTime.Now.Ticks;
		_reverseCounter = DateTime.MaxValue.Ticks;
		_lock = new SemaphoreSlim(1, 1);
		new Timer(ResetsCounters!, null, 0, 1000);
		ResetReverseCounter();
	}

	private static void ResetsCounters(object state)
	{
		ResetReverseCounter();
		ResetForwardCounter();
	}

	public static IEnumerable<long> GenerateDecreasingSequence(int length)
	{
		if (length < 0)
		{
			throw new ArgumentOutOfRangeException("length", "A sequence must have a positive number of items");
		}
		for (int i = 0; i < length; i++)
		{
			yield return NextDescendingUniqueValue;
		}
	}

	public static IEnumerable<long> GenerateIncreasingSequence(int length)
	{
		if (length < 0)
		{
			throw new ArgumentOutOfRangeException("length", "A sequence must have a positive number of items");
		}
		for (int i = 0; i < length; i++)
		{
			yield return NextAscendingUniqueValue;
		}
	}

	private static void ResetReverseCounter()
	{
		_lock.Wait();
		try
		{
			_reverseCounter = Math.Min(DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks, _reverseCounter) - 1;
		}
		finally
		{
			_lock.Release();
		}
	}

	private static void ResetForwardCounter()
	{
		_lock.Wait();
		try
		{
			_forwardCounter = Math.Max(DateTime.Now.Ticks, _forwardCounter) + 1;
		}
		finally
		{
			_lock.Release();
		}
	}
}
