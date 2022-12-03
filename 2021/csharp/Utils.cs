namespace Aoc.Solutions;
public static class Fn
{
    public static int CharToInt(char c) => c - '0';

    public static void Dump<T>(this T @this, bool indented = true) =>
        Console.WriteLine(@this.ToJson(indented));

    public static T Clone<T>(this T @this) =>
        System.Text.Json.JsonSerializer.Deserialize<T>(@this.ToJson())!;

    public static void DumpDict<K, V>(this Dictionary<K, V> @this) where K : notnull
    {
        Console.WriteLine(@this.Select(kvp => $"[{kvp.Key}]: {kvp.Value}").Join("\n"));
    }

    public static string ToJson<T>(this T @this, bool indented = true) =>
        System.Text.Json.JsonSerializer.Serialize(@this, options: new() { WriteIndented = indented });

    public static IEnumerable<(T Val, int Idx)> Indexed<T>(this IEnumerable<T> @this) => @this.Select((v, i) => (v, i));

    public static IEnumerable<List<T>> SplitAt<T>(this IEnumerable<T> @this, Func<T, bool> split)
    {
        var cur = new List<T>();
        foreach (var l in @this)
        {
            if (split(l))
            {
                yield return cur;
                cur = new();
            }
            else
            {
                cur.Add(l);
            }
        }
        if (cur.Any())
        {
            yield return cur;
        }
    }

    public static IEnumerable<(T Left, T Right)> AllPairs<T>(this IList<T> @this, bool flipped = true)
    {
        foreach (var i in Range(0, @this.Count))
        {
            foreach (var j in Range(0, @this.Count))
            {
                if (i != j)
                {
                    yield return (@this[i], @this[j]);
                    if (flipped)
                    {
                        yield return (@this[j], @this[i]);
                    }
                }
            }
        }
    }
}

public class DefaultDict<K, V> : Dictionary<K, V> where K : notnull
{
    private readonly Func<V> _default = () => default!;
    public DefaultDict()
    { }

    public DefaultDict(Dictionary<K, V> vals) : base(vals)
    { }

    /// <summary>
    /// Provide a function to generate a default value when not present
    /// </summary>
    /// <param name="defaultValueGenerator">e.g. () => new List()</param>
    public DefaultDict(Func<V> defaultValueGenerator)
    {
        _default = defaultValueGenerator;
    }
    public new V this[K key]
    {
        get
        {
            if (!TryGetValue(key, out V? val))
            {
                val = _default();
                Add(key, val);
            }
            return val;
        }
        set { base[key] = value; }
    }
}

public class Grid<T>
{
    readonly bool _diag = true;
    public T this[(int r, int c) val]
    {
        get => State[val.r][val.c];
        set => State[val.r][val.c] = value;
    }

    public Grid(List<List<T>> nums)
    {
        State = nums;
    }

    public Grid(List<List<T>> nums, bool diag)
    {
        _diag = diag;
        State = nums;
    }

    public IEnumerable<(int r, int c)> All()
    {
        foreach (var r in Range(0, Rows))
        {
            foreach (var c in Range(0, Cols))
            {
                yield return (r, c);
            }
        }
    }

    public IEnumerable<(int r, int c)> Surrounding((int r, int c) point)
    {
        foreach (var r in Range(point.r - 1, 3))
        {
            foreach (var c in Range(point.c - 1, 3))
            {
                if (r >= 0 && r < Rows && c >= 0 && c < Cols && (r != point.r || c != point.c)
                    && (_diag || r == point.r || c == point.c))
                {
                    yield return (r, c);
                }
            }
        }
    }

    public List<List<T>> State { get; }
    public int Rows => State.Count;
    public int Cols => State[0].Count;

    public override string ToString() =>
        State
        .Select(row => row.Select(r => r!.ToString()!).Join(""))
        .Join("\n");
}

public class DiagGrid<T> : Grid<T>
{
    public DiagGrid(List<List<T>> nums) : base(nums, true) { }
}

public class NoDiagGrid<T> : Grid<T>
{
    public NoDiagGrid(List<List<T>> nums) : base(nums, false) { }
}


public static class QueueExt
{
    public static T[] Dequeue<T>(this Queue<T> @this, int n)
    {
        var l = new List<T>(n);
        foreach (var i in Range(0, n))
        {
            l.Add(@this.Dequeue());
        }
        return l.ToArray();
    }
}