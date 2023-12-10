using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc.Runner;
public static class Helpers
{
    public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> items) => items.Select((t, i) => (t, i));


    public static IEnumerable<K> Choose<T, K>(this IEnumerable<T> values, Func<T, K?> chooser) =>
        values.Select(chooser).Where(x => x != null).Select(x => x!);

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

    public static string ToJson(this object s, bool indented = false) => System.Text.Json.JsonSerializer.Serialize(s, options: new()
    {
        WriteIndented = indented
    });

    public static void AddAll<T>(this HashSet<T> items, IEnumerable<T> others)
    {
        foreach (var i in others)
            items.Add(i);
    }

    public static DefaultDictionary<K, V> ToDefaultDict<K, V>(this Dictionary<K, V> dict) where K : notnull => ToDefaultDict(dict, () => default!);
    public static DefaultDictionary<K, V> ToDefaultDict<K, V>(this Dictionary<K, V> dict, Func<V> def) where K : notnull
    {
        var d = new DefaultDictionary<K, V>(def);
        foreach (var (k, v) in dict)
        {
            d[k] = v;
        }
        return d;
    }

    public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
    {
        readonly Func<TValue> _init;
        public DefaultDictionary(Func<TValue> init)
        {
            _init = init;
        }

        public DefaultDictionary()
        {
            _init = () => default!;
        }
        public new TValue this[TKey k]
        {
            get
            {
                if (!ContainsKey(k))
                    Add(k, _init());
                return base[k];
            }
            set => base[k] = value;
        }
    }
}