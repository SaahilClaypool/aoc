namespace Aoc.Solutions.Day06;
public record State(Dictionary<long, long> Fish)
{
    public State Next()
    {
        var nextState =
            Fish
            .SelectMany(kvp => kvp.Key switch
            {
                var k when k == 0 => new List<(long, long)> { (6, kvp.Value), (8, kvp.Value) },
                var k => new List<(long, long)> { (k - 1, kvp.Value) }
            })
            .GroupBy(fishLifeState => fishLifeState.Item1)
            .Select(fishLifeStateGroup => (fishLifeStateGroup.Key, fishLifeStateGroup.Select(v => v.Item2).Sum()))
            .ToDictionary(g => g.Key, g => g.Item2);

        return new(nextState);
    }
}

public class Day06 : Day
{
    public static State Parse(string line) =>
        line
            .Split(',')
            .Select(long.Parse)
            .GroupBy(v => v)
            .ToDictionary(v => v.Key, v => v.LongCount())
            .Then(dict => new State(dict));

    public override string SolveA(string input)
    {
        var state = Parse(input);
        foreach (var i in Range(0, 80))
        {
            state = state.Next();
        }
        return state.Fish.Values.Sum().ToString();
    }

    public override string SolveB(string input)
    {
        var state = Parse(input);
        foreach (var i in Range(0, 256))
        {
            state = state.Next();
        }
        return state.Fish.Values.Sum().ToString();
    }

    public Day06()
    {
        Tests = new()
        {
            new("A", "3,4,3,1,2", "5934", SolveA)
        };
    }
}