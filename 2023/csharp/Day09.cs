namespace Aoc.Solutions.Y2023;

public class Day09 : Day
{
    List<List<long>> Parse(string input) =>
        input.Lines().Select(line => line.Split(' ').Select(long.Parse).ToList()).ToList();

    long NextValue(List<long> input)
    {
        if (input.All(x => x == 0L))
            return 0;
        else
        {
            var differences = input.Pairs().Select(x => x.B - x.A).ToList();
            var nextRowValue = NextValue(differences);
            return input.Last() + nextRowValue;
        }
    }

    long PrevValue(List<long> input)
    {
        if (input.All(x => x == 0L))
            return 0;
        else
        {
            var differences = input.Pairs().Select(x => x.B - x.A).ToList();
            var prevValue = PrevValue(differences);
            return input.First() - prevValue;
        }
    }

    public override string SolveA(string input)
    {
        var lines = Parse(input);
        lines.Dump();

        var predictions = lines.Select(NextValue).ToList();
        predictions.Dump();

        return predictions.Sum().ToString();
    }

    // find the cycle (need the number of points until cycle starts + length of cycle)
    // then can count all the places that we will have wins
    // fold the wins together to find the fist win
    public override string SolveB(string input)
    {
        var lines = Parse(input);
        lines.Dump();

        var predictions = lines.Select(PrevValue).ToList();
        predictions.Dump();

        return predictions.Sum().ToString();
    }

    public Day09()
    {
        Tests = new()
        {

            new("a", SampleA, "114", SolveA),
            new("b", SampleA, "2", SolveB)
        };
    }

    static string SampleA = """
    0 3 6 9 12 15
    1 3 6 10 15 21
    10 13 16 21 30 45
    """;
}