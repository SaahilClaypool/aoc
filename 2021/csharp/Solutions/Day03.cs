using SharpResult.FunctionalExtensions;

namespace Aoc.Solutions;
public class Day03 : Day
{
    static char[] ParseLine(string line) => line.ToCharArray();
    static List<char[]> Parse(string input) => input.Split('\n').Select(ParseLine).ToList();

    public override string SolveA(string input)
    {
        return "TODO not working";
        var lines = Parse(input);
        var len = lines.First().Length;
        var gamma = new char[len];
        var eps = new char[len];
        foreach (var i in Enumerable.Range(0, len))
        {
            var col = lines.Select(l => l[i]).ToList();
            gamma[i] = col.Mode();
            eps[i] = gamma[i] == '1' ? '0' : '1';
        }
        var res = Convert.ToInt32(new string(gamma), 2) * Convert.ToInt32(new string(eps), 2);
        return res.ToString();
    }

    public override string SolveB(string input)
    {
        return "TODO not working";
        var lines = Parse(input);
        var len = lines.First().Length;

        var GetBits = (IEnumerable<char[]> curLines, int i) => curLines.Select(l => l[i]).ToList();

        var solve = (List<char> bits, Func<List<char>, List<bool>> criteria, List<char[]> intLines) =>
        {
            var output = criteria(bits);
            var res =  intLines.Zip(output).Where(z => z.Second == true).Select(z => z.First);
            return res;
        };

        var oxy = (List<char> bits) =>
        {
            var mostCommon = bits.Mode(def: '1');
            return bits.Select(b => b == mostCommon).ToList();
        };

        var co2 = (List<char> bits) =>
        {
            var mostCommon = bits.Mode(def: '1');
            var leastCommon = mostCommon == '1' ? '0' : '1';
            return bits.Select(b => b == leastCommon).ToList();
        };

        int? oxyRating = null;
        var oxyLines = lines;
        foreach (var i in Enumerable.Range(0, len))
        {
            oxyLines = solve(GetBits(oxyLines, i), oxy, oxyLines).ToList();
            if (oxyLines.Count == 1)
            {
                oxyRating = Convert.ToInt32(new string(oxyLines.First()), 2);
                break;
            }
        }

        int? co2Rating = null;
        var co2Lines = lines;
        foreach (var i in Enumerable.Range(0, len))
        {
            co2Lines = solve(GetBits(co2Lines, i), co2, co2Lines).ToList();
            if (co2Lines.Count == 1)
            {
                co2Rating = Convert.ToInt32(new string(co2Lines.First()), 2);
                break;
            }
        }
        return (co2Rating!.Value * oxyRating!.Value).ToString();
    }

    public Day03()
    {
        Tests = new()
        {
            new("A",
                @"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010",
            "198",
            SolveA
            ),
            new("B",
                @"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010",
            "230",
            SolveB
            )
        };
    }
}

static class LinqExt
{
    public static T Mode<T>(this IEnumerable<T> @this, T def = default!)
    {
        var counts = @this.GroupBy(t => t);
        var maxCount = counts.MaxBy(m => m.Count())!.Count();
        var withMaxCount = counts.Where(c => c.Count() == maxCount);
        if (withMaxCount.Count() == 1)
        {
            var mode = withMaxCount.First().Key;
            return mode;
        }
        else
        {
            return def;
        }
    }

    public static string Join<T>(this IEnumerable<T> t, string sep = ", ") => string.Join(sep, t.Select(x => x?.ToString()));
}