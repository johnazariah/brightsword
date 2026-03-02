using System;
using System.Diagnostics.CodeAnalysis;

namespace BrightSword.SwissKnife;

[ExcludeFromCodeCoverage]
internal class M<T>
{
	private readonly T _value;

	public M(T value)
	{
		_value = value;
	}

	public static implicit operator M<T>(T value)
	{
		return new M<T>(value);
	}

	public M<U> Bind<U>(Func<T, M<U>> func)
	{
		return func(_value);
	}

	public void Bind(Action<T> action)
	{
		action(_value);
	}
}
