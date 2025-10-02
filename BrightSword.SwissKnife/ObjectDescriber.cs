using System;
using System.Linq.Expressions;

#nullable disable
namespace BrightSword.SwissKnife;

public static class ObjectDescriber
{
  private static string GetName<TFunc>(Expression<TFunc> e)
  {
    if (e.Body is MemberExpression body1)
      return body1.Member.Name;
    if (e.Body is MethodCallExpression body2)
      return body2.Method.Name;
    throw new NotSupportedException("Cannot operate on given expression: " + (object) e.Body);
  }

  public static string GetName(Expression<Action> e) => ObjectDescriber.GetName<Action>(e);

  public static string GetName<TResult>(Expression<Func<TResult>> selector)
  {
    return ObjectDescriber.GetName<Func<TResult>>(selector);
  }

  public static string GetName<TArg, TResult>(Expression<Func<TArg, TResult>> selector)
  {
    return ObjectDescriber.GetName<Func<TArg, TResult>>(selector);
  }

  public static string GetName<TArg>(Expression<Action<TArg>> selector)
  {
    return ObjectDescriber.GetName<Action<TArg>>(selector);
  }
}
