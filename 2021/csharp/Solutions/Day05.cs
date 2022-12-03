using FluentAssertions;
namespace Aoc.Solutions.Day05;

public record Point(int X, int Y);
public record Line(Point A, Point B);
public record Board()
{
    public DefaultDict<Point, int> State = new();

    public void AddLine(Line l, bool diag = false)
    {
        var minX = Math.Min(l.A.X, l.B.X);
        var maxX = Math.Max(l.A.X, l.B.X);
        var minY = Math.Min(l.A.Y, l.B.Y);
        var maxY = Math.Max(l.A.Y, l.B.Y);
        if (l.A.Y == l.B.Y)
        {
            foreach (var r in Range(minX, maxX - minX + 1))
            {
                State[new(r, l.A.Y)] += 1;
            }
        }
        else if (l.A.X == l.B.X)
        {
            foreach (var c in Range(minY, maxY - minY + 1))
            {
                State[new(l.A.X, c)] += 1;
            }
        }
        else if (diag)
        {
            (maxX - minX).Should().Be(maxY - minY);
            var slope = (l.B.Y - l.A.Y) / (l.B.X - l.A.X);
    
            foreach (var i in Range(0, maxY - minY + 1))
            {
                if (slope > 0)
                {
                    State[new(minX + i, minY + i)] += 1;
                }
                else
                {
                    State[new(minX + i, maxY - i)] += 1;
                }
            }
        }
    }
}

public class Day05 : Day
{
    public static Line ParseLine(string line)
    {
        var parts = line.Split(' ');
        var firstPair = parts[0];
        var lastPair = parts[^1];
        var parsePoint = (string str) => new Point(int.Parse(str.Split(',')[0]), int.Parse(str.Split(',')[1]));
        return new(parsePoint(firstPair), parsePoint(lastPair));
    }

    public static List<Line> ParseInput(string input) =>
        input
        .Split('\n')
        .Select(ParseLine)
        .ToList();

    public override string SolveA(string input)
    {
        var lines = ParseInput(input);
        var board = new Board();
        foreach (var line in lines)
        {
            board.AddLine(line);
        }
        return board.State.Values.Where(v => v >= 2).Count().ToString();
    }

    public override string SolveB(string input)
    {
        var lines = ParseInput(input);
        var board = new Board();
        foreach (var line in lines)
        {
            board.AddLine(line, diag: true);
        }
        // Format(board.State);
        return board.State.Values.Where(v => v >= 2).Count().ToString();
    }

    private void Format(DefaultDict<Point, int> state)
    {
        var minX = state.Keys.Select(p => p.X).Min();
        var maxX = state.Keys.Select(p => p.X).Max();
        var minY = state.Keys.Select(p => p.Y).Min();
        var maxY = state.Keys.Select(p => p.Y).Max();

        foreach (var y in Range(minY, maxY - minY + 1))
        {
            foreach (var x in Range(minX, maxX - minX + 1))
            {
                if (state[new(x, y)] > 0)
                {
                    Console.Write($"{state[new(x, y)]}");
                }
                else
                {
                    Console.Write($".");
                }
            }
        }
    }

    public Day05()
    {
        Tests = new()
        {
            new(
            "A",
            @"0,9 -> 5,9
8,0 -> 0,8
9,4 -> 3,4
2,2 -> 2,1
7,0 -> 7,4
6,4 -> 2,0
0,9 -> 2,9
3,4 -> 1,4
0,0 -> 8,8
5,5 -> 8,2",
           "5",
           SolveA),
            new(
            "B",
            @"0,9 -> 5,9
8,0 -> 0,8
9,4 -> 3,4
2,2 -> 2,1
7,0 -> 7,4
6,4 -> 2,0
0,9 -> 2,9
3,4 -> 1,4
0,0 -> 8,8
5,5 -> 8,2",
           "12",
           SolveB)
        };
    }
}