using System;
using System.Linq.Expressions;

namespace BrightSword.SwissKnife
{
    public static class ObjectDescriber
    {
        private static string GetName<TFunc>(Expression<TFunc> e)
        {
            return e.Body switch
            {
                MemberExpression m => m.Member.Name,
                MethodCallExpression m => m.Method.Name,
                _ => throw new NotSupportedException($"Cannot operate on given expression: {e.Body}")
            };
        }

        public static string GetName(Expression<Action> e) => GetName<Action>(e);

        public static string GetName<TResult>(Expression<Func<TResult>> selector) => GetName<Func<TResult>>(selector);

        public static string GetName<TArg, TResult>(Expression<Func<TArg, TResult>> selector) => GetName<Func<TArg, TResult>>(selector);

        public static string GetName<TArg>(Expression<Action<TArg>> selector) => GetName<Action<TArg>>(selector);
    }
}
