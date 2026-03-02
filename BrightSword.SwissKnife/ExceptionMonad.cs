using System;
using System.Collections.Generic;
using System.Linq;

namespace BrightSword.SwissKnife;

public static class ExceptionMonad
{
	public static Func<ExceptionMonad<TArgument>, IEnumerable<ExceptionMonad<TResult>>> MakeSafe<TArgument, TResult>(this Func<TArgument, IEnumerable<TResult>> function)
	{
		return ExceptionMonad<TResult>.Unit(function);
	}

	public static Func<ExceptionMonad<TArgument>, ExceptionMonad<TResult>> MakeSafe<TArgument, TResult>(this Func<TArgument, TResult> function)
	{
		return ExceptionMonad<TResult>.Unit(function);
	}

	public static Func<ExceptionMonad<TArgument>, int, ExceptionMonad<TResult>> MakeSafe<TArgument, TResult>(this Func<TArgument, int, TResult> function)
	{
		return ExceptionMonad<TResult>.Unit(function);
	}

	public static Func<IEnumerable<ExceptionMonad<TResult>>> MakeSafe<TResult>(this Func<IEnumerable<TResult>> function)
	{
		return ExceptionMonad<TResult>.Unit(function);
	}

	public static Func<ExceptionMonad<TResult>> MakeSafe<TResult>(this Func<TResult> function)
	{
		return ExceptionMonad<TResult>.Unit(function);
	}

	public static Action MakeSafe(this Action action)
	{
		return ExceptionMonad<object>.Unit(action);
	}

	public static Action<ExceptionMonad<TArgument>> MakeSafe<TArgument>(this Action<TArgument> action)
	{
		return ExceptionMonad<TArgument>.Unit(action);
	}

	public static void SafeCall<TResult>(this Func<TResult> function, Action<TResult> action)
	{
		function.MakeSafe()().Bind(action);
	}

	public static ExceptionMonad<TResult> SafeCall<TArgument, TResult>(this Func<TArgument> first, Func<TArgument, TResult> second)
	{
		return second.MakeSafe()(first.MakeSafe()());
	}

	public static IEnumerable<ExceptionMonad<TResult>> SelectMany<TArgument, TResult>(this IEnumerable<ExceptionMonad<TArgument>> _this, Func<TArgument, IEnumerable<TResult>> function)
	{
		return _this.SelectMany((ExceptionMonad<TArgument> _) => _.BindMany(function));
	}

	public static IEnumerable<ExceptionMonad<TResult>> Select<TArgument, TResult>(this IEnumerable<ExceptionMonad<TArgument>> _this, Func<TArgument, TResult> function)
	{
		return _this.Select((ExceptionMonad<TArgument> _) => _.Bind(function));
	}

	public static IEnumerable<ExceptionMonad<TResult>> Select<TArgument, TResult>(this IEnumerable<ExceptionMonad<TArgument>> _this, Func<TArgument, int, TResult> function)
	{
		return _this.Select((ExceptionMonad<TArgument> _item, int _index) => _item.Bind(function, _index));
	}
}

public sealed class ExceptionMonad<T>
{
	private T? Result { get; set; }

	private Exception? Exception { get; set; }

	public bool IsException => Exception != null;

	private ExceptionMonad(T result)
	{
		Result = result;
	}

	private ExceptionMonad(Exception exception)
	{
		Exception = exception;
	}

	public static implicit operator ExceptionMonad<T>(T value)
	{
		return new ExceptionMonad<T>(value);
	}

	public static implicit operator ExceptionMonad<T>(Exception exception)
	{
		return new ExceptionMonad<T>(exception);
	}

	public static Func<ExceptionMonad<TResult>> Unit<TResult>(Func<TResult> function)
	{
		return delegate
		{
			try
			{
				return function();
			}
			catch (Exception ex)
			{
				return ex;
			}
		};
	}

	public static Action Unit(Action action)
	{
		return delegate
		{
			try
			{
				action();
			}
			catch
			{
			}
		};
	}

	public static Action<ExceptionMonad<TArgument>> Unit<TArgument>(Action<TArgument> action)
	{
		return delegate(ExceptionMonad<TArgument> _)
		{
			if (_.Exception == null)
			{
				Action action2 = delegate
				{
					action(_.Result!);
				};
				Unit(action2)();
			}
		};
	}

	public static Func<IEnumerable<ExceptionMonad<TResult>>> Unit<TResult>(Func<IEnumerable<TResult>> function)
	{
		return delegate
		{
			try
			{
				List<TResult> source = function().ToList();
				return source.Select((TResult result) => new ExceptionMonad<TResult>(result));
			}
			catch (Exception exception)
			{
				return Enumerable.Repeat(new ExceptionMonad<TResult>(exception), 1);
			}
		};
	}

	public static Func<ExceptionMonad<TArgument>, ExceptionMonad<TResult>> Unit<TArgument, TResult>(Func<TArgument, TResult> function)
	{
		return delegate(ExceptionMonad<TArgument> _)
		{
			if (_.Exception != null)
			{
				return _.Exception;
			}
			Func<TResult> function2 = () => function(_.Result!);
			return Unit(function2)();
		};
	}

	public static Func<ExceptionMonad<TArgument>, int, ExceptionMonad<TResult>> Unit<TArgument, TResult>(Func<TArgument, int, TResult> function)
	{
		return delegate(ExceptionMonad<TArgument> _argument, int _index)
		{
			if (_argument.Exception != null)
			{
				return _argument.Exception;
			}
			Func<TResult> function2 = () => function(_argument.Result!, _index);
			return Unit(function2)();
		};
	}

	public static Func<ExceptionMonad<TArgument>, IEnumerable<ExceptionMonad<TResult>>> Unit<TArgument, TResult>(Func<TArgument, IEnumerable<TResult>> function)
	{
		return delegate(ExceptionMonad<TArgument> _)
		{
			if (_.Exception != null)
			{
				return Enumerable.Repeat(new ExceptionMonad<TResult>(_.Exception), 1);
			}
			Func<IEnumerable<TResult>> function2 = () => function(_.Result!);
			return Unit(function2)();
		};
	}

	public IEnumerable<ExceptionMonad<U>> BindMany<U>(Func<T, IEnumerable<U>> func)
	{
		return (Exception == null) ? Unit(func)(Result!) : Enumerable.Repeat(new ExceptionMonad<U>(Exception), 1);
	}

	public void Bind(Action<T> action)
	{
		if (Exception == null)
		{
			Unit(action)(Result!);
		}
	}

	public ExceptionMonad<U> Bind<U>(Func<T, U> func)
	{
		return (Exception == null) ? Unit(func)(Result!) : new ExceptionMonad<U>(Exception);
	}

	public ExceptionMonad<U> Bind<U>(Func<T, int, U> func, int index)
	{
		return (Exception == null) ? Unit(func)(Result!, index) : new ExceptionMonad<U>(Exception);
	}

	public void Do(Action<T> action)
	{
		if (Exception != null)
		{
			throw Exception;
		}
		action(Result!);
	}

	public T GetValueOrDefault(T defaultValue = default!)
	{
		return (Exception == null) ? Result! : defaultValue;
	}

	public Exception? GetException()
	{
		return Exception;
	}
}
