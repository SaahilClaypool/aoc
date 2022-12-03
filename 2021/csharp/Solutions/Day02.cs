using SharpResult.FunctionalExtensions;

namespace Aoc.Solutions;
public class Day02 : Day
{
    record Pos(int H, int D, int A);
    record Command(string C, int X)
    {
        public static Command FromString(string s)
        {
            var parts = s.Split();
            return new(parts[0], int.Parse(parts[1]));
        }
    }

    public override string SolveA(string input)
    {
        var pos = new Pos(0, 0, 0);
        foreach (var comm in input.Split('\n').Select(Command.FromString))
        {
            pos = comm switch
            {
                ("forward", var x) => pos with { H = pos.H + x },
                ("up", var x) => pos with { D = pos.D - x },
                ("down", var x) => pos with { D = pos.D + x },
                _ => throw new Exception()
            };
        }
        return (pos.H * pos.D).ToString();
    }

    public override string SolveB(string input)
    {
        var pos = new Pos(0, 0, 0);
        foreach (var comm in input.Split('\n').Select(Command.FromString))
        {
            pos = comm switch
            {
                ("forward", var x) => pos with { H = pos.H + x, D = pos.D + x * pos.A },
                ("up", var x) => pos with { A = pos.A - x },
                ("down", var x) => pos with { A = pos.A + x },
                _ => throw new Exception()
            };
        }
        return (pos.H * pos.D).ToString();
    }

    public Day02()
    {
        Tests = new()
        {
            new("A", @"forward 5
down 5
forward 8
up 3
down 8
forward 2", "150", SolveA)
        };
    }
}