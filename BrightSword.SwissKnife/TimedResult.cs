using System;

namespace BrightSword.SwissKnife;

public struct TimedResult<T>
{
	public T Result { get; private set; }

	public TimeSpan ElapsedTime { get; private set; }

	public TimedResult(T result, TimeSpan elapsedTime)
	{
		this = default(TimedResult<T>);
		ElapsedTime = elapsedTime;
		Result = result;
	}
}
