using System;
using System.Diagnostics;

namespace BrightSword.SwissKnife;

public static class TimedOperationExtensions
{
	public static TimeSpan Time(this Action _this, ITimedOperationObserver? observer = null)
	{
		Stopwatch stopwatch = new Stopwatch();
		observer = observer ?? new TraceObserver();
		stopwatch.Stop();
		stopwatch.Start();
		try
		{
			observer.Started();
			_this();
			observer.Succeeded();
		}
		catch (Exception exception)
		{
			observer.FailedWithException(exception);
			throw;
		}
		finally
		{
			stopwatch.Stop();
			observer.Completed(stopwatch.Elapsed);
		}
		return stopwatch.Elapsed;
	}

	public static TimedResult<T> Time<T>(this Func<T> _this, ITimedOperationObserver? observer = null)
	{
		Stopwatch stopwatch = new Stopwatch();
		observer = observer ?? new TraceObserver();
		stopwatch.Stop();
		stopwatch.Start();
		T result;
		try
		{
			observer.Started();
			result = _this();
			observer.Succeeded();
		}
		catch (Exception exception)
		{
			observer.FailedWithException(exception);
			throw;
		}
		finally
		{
			stopwatch.Stop();
			observer.Completed(stopwatch.Elapsed);
		}
		return new TimedResult<T>(result, stopwatch.Elapsed);
	}
}
