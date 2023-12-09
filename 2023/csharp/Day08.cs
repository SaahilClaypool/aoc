global using System.Text.RegularExpressions;
using System.Numerics;

namespace Aoc.Solutions.Y2023;

public class Day08 : Day
{
    public override string SolveA(string input)
    {
        var inp = Parse(input);
        var start = "AAA";
        var end = "ZZZ";
        var path = Walk(inp, start, new() { end });
        return path.Count().ToString();
    }

    // find the cycle (need the number of points until cycle starts + length of cycle)
    // then can count all the places that we will have wins
    // fold the wins together to find the fist win
    public override string SolveB(string input)
    {
        var inp = Parse(input);
        var (path, nodes) = inp;
        var starts = nodes.Keys.Where(x => x.EndsWith('A')).ToList();
        var ends = nodes.Keys.Where(x => x.EndsWith('Z')).ToHashSet();
        var pathLengths = starts.Select(start => Walk(inp, start, ends)).Select(x => (long)x.Count());
        var fullLCM = pathLengths.Aggregate(1L, LCM);
        return fullLCM.ToString();
    }

    record Input(string Directions, Dictionary<string, (string Left, string Right)> Nodes);

    Input Parse(string input)
    {
        var path = input.Lines()[0];
        var nodes = input.Lines().Skip(2)
            .Select(x =>
            {
                var key = x.Split(" = ")[0].Trim();
                var values = Regex.Matches(x.Split(" = ")[1], @"\w+").Select(x => x.Value).ToList();
                return (key, (values[0], values[1]));
            }).ToDictionary();
        return new(path, nodes);
    }

    IEnumerable<string> Walk(Input input, string start, HashSet<string> end)
    {
        int i = 0;
        var cur = start;
        while (!end.Contains(cur)){
            cur = input.Directions[i] == 'L' ? input.Nodes[cur].Left : input.Nodes[cur].Right;
            yield return cur;
            i = (i + 1) % input.Directions.Length;
        }
    }

    public long LCM(long a, long b) => a * b / GCD(a, b);
    public long GCD(long a, long b) => b == 0 ? a : GCD(b, a % b);

    public Day08()
    {
        Tests = new()
        {

            new("a1", SampleA1, "2", SolveA),
            new("a2", SampleA1, "6", SolveB),
            new("b1", SampleB, "6", SolveB),
            new("lcm", "2 & 3", "6", _ => LCM(2, 3).ToString())
        };
    }


    string SampleA1 = """
        RL

        AAA = (BBB, CCC)
        BBB = (DDD, EEE)
        CCC = (ZZZ, GGG)
        DDD = (DDD, DDD)
        EEE = (EEE, EEE)
        GGG = (GGG, GGG)
        ZZZ = (ZZZ, ZZZ)
        """;

    string SampleB = """
        LR

        11A = (11B, XXX)
        11B = (XXX, 11Z)
        11Z = (11B, XXX)
        22A = (22B, XXX)
        22B = (22C, 22C)
        22C = (22Z, 22Z)
        22Z = (22B, 22B)
        XXX = (XXX, XXX)
        """;
}