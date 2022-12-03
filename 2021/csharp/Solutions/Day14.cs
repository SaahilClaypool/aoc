namespace Aoc.Solutions.Day14;

public class Day14 : Day
{
    public (string template, Dictionary<string, char> rules) Parse(string input)
    {
        var sections =
            input
            .Split('\n')
            .SplitAt(l => l.Trim().Length == 0).ToList();
        var template = sections[0][0];
        var rules =
            sections[1]
            .Select(line =>
            {
                var parts = line.Split(' ');
                return (key: parts[0], val: parts[^1][0]);
            })
            .ToDictionary(p => p.key, p => p.val);

        return (template, rules);
    }

    public override string SolveA(string input)
    {
        var (template, rules) = Parse(input);
        var result =Solve(template, rules, iterations: 10);
        return result.ToString();
    }

    public override string SolveB(string input)
    {
        var (template, rules) = Parse(input);
        var result = Solve(template, rules, iterations: 40);

        return result.ToString();
    }

    // idea:
    // keep track of all pairs in the input
    // each step, add the count of new pairs
    private static long Solve(string template, Dictionary<string, char> rules, int iterations)
    {
        var letterCounts = new DefaultDict<char, long>();
        var pairs = new DefaultDict<string, long>();

        for (var i = 0; i < template.Length - 1; i++)
        {
            var pair = template[i..(i + 2)];
            pairs[pair] += 1;
            letterCounts[template[i]] += 1;
        }
        letterCounts[template[^1]] += 1;
        // letterCounts.Dump(false);

        for (var i = 0; i < iterations; i++)
        {
            var nextPairs = new DefaultDict<string, long>(pairs.ToDictionary(p => p.Key, p => p.Value));
            foreach (var pair in pairs.Keys)
            {
                var pairCount = pairs[pair];
                if (pairCount == 0)
                {
                    continue;
                }
                if (rules.TryGetValue(pair, out var c))
                {
                    nextPairs[pair] -= pairCount;
                    var left = $"{pair[0]}{c}";
                    var right = $"{c}{pair[1]}";

                    nextPairs[left] += pairCount;
                    nextPairs[right] += pairCount;
                    letterCounts[c] += pairCount;
                }
            }
            pairs = nextPairs;
            var maxl = letterCounts.MaxBy(k => k.Value);
            var minl = letterCounts.MinBy(k => k.Value);
            // letterCounts.Dump(false);
            // pairs.Dump(false);
        }

        var maxL = letterCounts.MaxBy(k => k.Value);
        var minL = letterCounts.MinBy(k => k.Value);
        return maxL.Value - minL.Value;
    }

    public Day14()
    {
        Tests = new()
        {
            new("A", @"NNCB

CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C", "1588", SolveA)
        };
    }
}