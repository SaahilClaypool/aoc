using Aoc2022.GridType;

namespace Aoc2022;


public partial class Day14 : Day
{
    record Map(List<List<char>> Data) : Grid<char>(Data, Diagonal: true)
    {
        public bool Occupied(Pos p) => this[p] != ' ';
        public bool Empty(Pos p) => this[p] == ' ';
        public static IEnumerable<Pos> Moves(Pos p) => new[]
        {
            p with { Row = p.Row + 1 },
            p with { Col = p.Col - 1, Row = p.Row + 1 },
            p with { Col = p.Col + 1, Row = p.Row + 1 },
        };

        public bool DropGrain(Pos start)
        {
            var current = start;
            if (!InBounds(start))
            {
                return true;
            }
            var move = Moves(start).Where(p => !InBounds(p) || !Occupied(p)).FirstOrDefault();
            if (move == default)
            {
                this[current] = '+';
                return false;
            }
            else
            {
                return DropGrain(move);
            }
        }
    }

    public override string SolveA(string input)
    {
        var grid = Parse(input);
        var grains = 0;
        while (!grid.DropGrain(new(0, 500)))
        {
            grains++;
        }
        Log(grid);
        return grains.ToString();
    }

    public override string SolveB(string input)
    {
        var grid = Parse(input);
        var offset = 1000;
        // copy padding to mimic infinite grid
        var padding = Enumerable.Range(0, offset).Select(_ => ' ');
        grid = new Map(
            grid.Data.Select(
                row => padding.Concat(row).Concat(padding).ToList()
            ).ToList()
        );
        // add floor
        grid.Data.Add(Enumerable.Range(0, grid.Cols).Select(_ => ' ').ToList());
        grid.Data.Add(Enumerable.Range(0, grid.Cols).Select(_ => ' ').ToList());
        for (var i = 0; i < grid.Cols; i++)
        {
            grid[new(grid.Rows - 1, i)] = '#';
        }
        var grains = 0;
        var start = new Pos(0, offset + 500);
        while (grid.Empty(start))
        {
            grid.DropGrain(start);
            grains++;
        }
        return grains.ToString();
    }

    /// <summary>
    /// grid where each spot indicates the next collision
    /// </summary>
    private static Map Parse(string input)
    {
        var re = PosRegex();
        var lines = input.Lines()
            .Select(line =>
                re
                .Matches(line)
                .Select(match =>
                    new Pos(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[1].Value))).ToList()
            )
            .Select(points => new Line(points))
            .ToList();

        var maxX = lines.SelectMany(l => l.Points).Max(p => p.Col);
        var maxY = lines.SelectMany(l => l.Points).Max(p => p.Row);

        var items = Enumerable.Range(0, maxY + 1).Select(r =>
            Enumerable.Range(0, maxX + 1).Select(_ => ' ').ToList()
        ).ToList();
        var grid = new Map(items);

        foreach (var line in lines)
        {
            var startPair = (new Pos(0, 0), line.Points.First());
            var pairs = line.Points.Rolling(startPair, (lastPair, cur) => (lastPair.Item2, cur)).Skip(1);
            foreach (var pair in pairs)
            {
                var minR = int.Min(pair.Item1.Row, pair.Item2.Row);
                var maxR = int.Max(pair.Item1.Row, pair.Item2.Row);
                var minC = int.Min(pair.Item1.Col, pair.Item2.Col);
                var maxC = int.Max(pair.Item1.Col, pair.Item2.Col);
                // vertical
                if (maxC == minC)
                {
                    for (var r = minR; r <= maxR; r++)
                    {
                        grid[new(r, maxC)] = '#';
                    }
                }
                else
                {
                    for (var c = minC; c <= maxC; c++)
                    {
                        grid[new(maxR, c)] = '#';
                    }
                }
            }
        }

        return grid;
    }

    record Line(List<Pos> Points);

    public Day14()
    {
        Tests = new()
        {
            new("A",
            """
            498,4 -> 498,6 -> 496,6
            503,4 -> 502,4 -> 502,9 -> 494,9
            """,
            "24",
            SolveA),
            new("B",
            """
            498,4 -> 498,6 -> 496,6
            503,4 -> 502,4 -> 502,9 -> 494,9
            """,
            "93",
            SolveB)
        };
    }

    [GeneratedRegex("(\\d+),(\\d+)")]
    private static partial Regex PosRegex();
}