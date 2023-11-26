global using Aoc.Runner;
global using static Aoc.Solutions.Y2023.Ext;

namespace Aoc.Solutions.Y2023;

public class Day03 : Day
{
    (string Left, string Right) Parse(string line)
    {
        var midpoint = line.Length / 2;
        // range is exclusive - inclusive in f#
        return (line[0..midpoint], line[midpoint..]);
    }

    int Priority(char letter) =>
        char.IsLower(letter) switch
        {
            true => 1 + letter - 'a',
            false => 1 + letter - 'A' + 26,
        };

    char Overlap(IEnumerable<string> blocks)
    {
        var firstBlock = blocks.First();
        var restBlock = blocks.Skip(1);
        return restBlock
            .Aggregate(firstBlock.AsEnumerable(), (state, next) => state.Intersect(next))
            .First();
    }

    public override string SolveA(string input) =>
        input
            .Split('\n')
            .Select(
                line =>
                    line.Pipe(Parse)
                        .Pipe(ruck => new[] { ruck.Left, ruck.Right })
                        .Pipe(Overlap)
                        .Pipe(Priority)
            )
            .Sum()
            .ToString();

    public override string SolveB(string input) =>
        input.Split('\n')
            .Chunk(3)
            .Select(group => group.Pipe(Overlap).Pipe(Priority))
            .Sum()
            .ToString();
}
