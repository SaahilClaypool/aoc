namespace Aoc.Solutions.Day08;

record Signal(List<string> Patterns);
record Line(Signal In, Signal Out);

public class Day08 : Day
{
    static Dictionary<string, int> Lookup = new()
    {
        ["abcefg"] = 0,
        ["cf"] = 1,
        ["acdeg"] = 2,
        ["acdfg"] = 3,
        ["bcdf"] = 4,
        ["abdfg"] = 5,
        ["abdefg"] = 6,
        ["acf"] = 7,
        ["abcdefg"] = 8,
        ["abcdfg"] = 9,
    };

    Signal ParseDigits(string s) => new(s
        .Split(" ")
        .Select(w => new string(w.OrderBy(c => c).ToArray()))
        .Where(d => d.Length > 0)
        .ToList());

    Line ParseLine(string input)
    {
        var els = input
            .Split('|')
            .Select(ParseDigits)
            .ToList();
        Log($"{input} {els.ToJson(false)}");
        return new(els[0], els[1]);
    }

    List<Line> Parse(string input) =>
        input.Split('\n').Select(ParseLine).ToList();

    public override string SolveA(string inputString)
    {
        var nums = Parse(inputString);
        var knownLengths = Lookup
            .Keys
            .GroupBy(k => k.Length)
            .Where(k => k.Count() == 1)
            .Select(k => k.Key)
            .ToHashSet();

        return nums
            .SelectMany(n => n.Out.Patterns)
            .Where(p => knownLengths.Contains(p.Length))
            .Count()
            .ToString();
    }

    int SolveLine(Line line)
    {
        var patterns = line.In.Patterns;
        var ofLength = (int l) => patterns.Where(p => p.Length == l);

        var one = ofLength(2).First();
        var seven = ofLength(3).First();
        var four = ofLength(4).First();
        var eight = ofLength(7).First();

        // Length 6: 6 9 0
        // 6: only one without part of one
        var six = ofLength(6).First(p => one.Except(p).Any());
        // 0: missing 4's cross
        var zero = ofLength(6).First(p => p != six && four.Except(p).Any());
        // 9: whatever is left
        var nine = ofLength(6).First(p => p != six && p != zero);
        
        // Length 5: 5, 2, 3
        // two has more than 9 - 5 & 3 do not
        var two = ofLength(5).First(p => p.Union(nine).Count() == 7);
        // five is just 6 missing one
        var five = ofLength(5).First(p => six.Except(p).Count() == 1 && p != two);
        var three = ofLength(5).First(p => p != two && p != five);

        
        var map = new List<string>() { zero, one, two, three, four, five, six, seven, eight, nine }
            .Indexed()
            .ToDictionary(
                v => v.Val,
                v => v.Idx.ToString()
            );

        var outputString = line.Out.Patterns.Select(p => map[p]).Join("");

        return int.Parse(outputString);
    }

    public override string SolveB(string inputString)
    {
        var nums = Parse(inputString);
        var solved = nums.Select(SolveLine).Sum();
        return solved.ToString();
    }

    public Day08()
    {
        Tests = new()
        {
            new("A", @"be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce", "26", SolveA),
            new("B Simple",
            "acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf",
            "5353", SolveB),
            new("B", @"be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce", "61229", SolveB)
        };
    }
}