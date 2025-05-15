using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReflexPlus.Extensions
{
    internal static class EnumerableExtensions
    {
        private static readonly ConcurrentDictionary<Type, Func<IEnumerable, object>> EnumerableCastDelegates = new();

        private static readonly MethodInfo EnumerableCastMethodInfo = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast))!;

        public static object CastDynamic(this IEnumerable source, Type target)
        {
            var castDelegate = EnumerableCastDelegates.GetOrAdd(target,
                t => EnumerableCastMethodInfo
                    .MakeGenericMethod(t)
                    .CreateDelegate<Func<IEnumerable, object>>());

            return castDelegate(source);
        }

        public static IEnumerable<T> Reversed<T>(this IList<T> items)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }
    }
}