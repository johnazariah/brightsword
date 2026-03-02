using System;
using System.Diagnostics.CodeAnalysis;

namespace BrightSword.SwissKnife;

[ExcludeFromCodeCoverage]
internal class Black<TCash>
{
	private readonly TCash _value;

	private Black(TCash value)
	{
		_value = value;
	}

	public static implicit operator Black<TCash>(TCash value)
	{
		return new Black<TCash>(value);
	}

	public Black<TSomething> Buy<TSomething>(Func<TCash, Black<TSomething>> shopForAnythingFunc)
	{
		return shopForAnythingFunc(_value);
	}
}
