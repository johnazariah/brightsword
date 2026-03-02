#nullable disable
using System;
using System.Diagnostics.CodeAnalysis;

namespace BrightSword.SwissKnife;

[ExcludeFromCodeCoverage]
internal class Nullable<T> where T : class
{
	private readonly T _value;

	private Nullable(T value)
	{
		_value = value;
	}

	public static implicit operator Nullable<T>(T value)
	{
		return new Nullable<T>(value);
	}

	public Nullable<U> Maybe<U>(Func<T, Nullable<U>> func) where U : class
	{
		return (_value == null) ? null : func(_value);
	}
}

[ExcludeFromCodeCoverage]
internal static class Nullable
{
	public static Nullable<U> Maybe<T, U>(this T value, Func<T, Nullable<U>> func) where T : class where U : class
	{
		return (value == null) ? null : func(value);
	}
}
