using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.Runner;
public static class Helpers
{
    public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> items) => items.Select((t, i) => (t, i));

    public static IEnumerable<(T A, T B)> Pairs<T>(this IEnumerable<T> items)
    {
        T past = default!;
        var first = false;
        foreach (var i in items)
        {
            if (!first)
            {
                first = true;
                past = i;
                continue;
            }
            yield return (past, i);
            past = i;
        }
    }

    public static IEnumerable<R> Rolling<T, R>(this IEnumerable<T> items, R seed, Func<R, T, R> select)
    {
        var last = seed;
        foreach (var i in items)
        {
            last = select(last, i);
            yield return last;
        }
    }

    public static IEnumerable<T> Rolling<T>(this IEnumerable<T> items, Func<T, T, T> select)
    {
        T last = default!;
        var first = false;
        foreach (var i in items)
        {
            if (!first)
            {
                first = true;
                last = i;
                continue;
            }
            last = select(last, i);
            yield return last;
        }
    }

    public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> items, Func<T, bool> condition)
    {
        foreach (var i in items)
        {
            if (condition(i))
            {
                yield return i;
            }
            else
            {
                yield break;
            }
        }
    }

    public static string[] Lines(this string s) => s.Split('\n');
}