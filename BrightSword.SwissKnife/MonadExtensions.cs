using System;

namespace BrightSword.SwissKnife
{
    public static class MonadExtensions
    {
        public static TResult Maybe<T, TResult>(
            this T This,
            Func<T, TResult> func,
            TResult defaultResult = default)
            where T : class => This is null ? defaultResult : func(This);

        public static T Maybe<T>(this T This, Action<T> action) where T : class
        {
            if (This is not null)
            {
                action(This);
            }
            return This;
        }

        public static TResult When<T, TResult>(this T This, Func<T, bool> predicate, Func<T, TResult> func, TResult defaultResult = default) where T : class
            => This.Maybe(_ => predicate(_) ? func(_) : defaultResult, defaultResult);

        public static T When<T>(this T This, Func<T, bool> predicate, Action<T> action) where T : class
            => This.Maybe(_ =>
            {
                if (predicate(_))
                {
                    action(_);
                }

                return _;
            });

        public static TResult Unless<T, TResult>(this T This, Func<T, bool> predicate, Func<T, TResult> func, TResult defaultResult = default) where T : class
            => This.Maybe(_ => !predicate(_) ? func(_) : defaultResult, defaultResult);

        public static T Unless<T>(this T This, Func<T, bool> predicate, Action<T> action) where T : class
            => This.Maybe(_ =>
            {
                if (!predicate(_))
                {
                    action(_);
                }

                return _;
            });
    }
}
