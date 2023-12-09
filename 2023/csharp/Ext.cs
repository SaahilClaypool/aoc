namespace Aoc.Solutions.Y2023;

public static class Ext
{
    public static K Pipe<T, K>(this T el, Func<T, K> func) => func(el);
    public static Func<T, Z> Compose<T, K, Z>(this Func<T, K> f, Func<K, Z> func) => x => f(x).Pipe(func);
    public static string ToJson(this object o) => System.Text.Json.JsonSerializer.Serialize(o);
    public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<(K, V)> tuples)
    where K : notnull =>
        tuples.ToDictionary(k => k.Item1, k => k.Item2);
}
