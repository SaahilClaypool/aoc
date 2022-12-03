namespace Aoc.Solutions.Day13;

public class Day13 : Day
{
    static int CharToInt(char c) => c - '0';

    public (HashSet<(int x, int y)> points, List<(bool y, int val)> folds) Parse(string input)
    {
        var parts = input.Split('\n').SplitAt(l => l.Trim().Length == 0).ToList();
        var points =
            parts[0]
            .Select(line => line.Split(',').Select(int.Parse).ToList())
            .Select(nums => (nums[0], nums[1]))
            .ToHashSet();
        var folds =
            parts[1]
            .Select(line =>
            {
                var (left, right) = (line.Split('=')[0], line.Split('=')[1]);
                var y = left.Last() == 'y';
                var val = int.Parse(right);
                return (y, val);
            })
            .ToList();
        return (points, folds);
    }

    public HashSet<(int x, int y)> FoldX(HashSet<(int x, int y)> points, int x)
    {
        var transform = ((int x, int y) point) =>
            point.x > x ?
                // when to the right, move
                (x - (point.x - x), point.y) :
                (point.x, point.y);

        return points.Select(transform).ToHashSet();
    }

    public HashSet<(int x, int y)> FoldY(HashSet<(int x, int y)> points, int y)
    {
        var transform = ((int x, int y) point) =>
            point.y > y ?
                // when to the right, move
                (point.x, y - (point.y - y)) :
                (point.x, point.y);

        return points.Select(transform).ToHashSet();
    }

    public string Format(HashSet<(int x, int y)> points)
    {
        var minX = points.Select(p => p.x).Min();
        var maxX = points.Select(p => p.x).Max();
        var minY = points.Select(p => p.y).Min();
        var maxY = points.Select(p => p.y).Max();

        var s = new System.Text.StringBuilder();
        foreach (var y in Range(minY, maxY - minY + 1))
        {
            foreach (var x in Range(minX, maxX - minX + 1))
            {
                if (points.Contains((x, y)))
                {
                    s.Append('#');
                }
                else
                {
                    s.Append('.');
                }
            }
            s.Append('\n');
        }
        return s.ToString();
    }

    public override string SolveA(string input)
    {
        var (points, folds) = Parse(input);
        foreach (var (y, val) in folds.Take(1))
        {
            if (y)
            {
                points = FoldY(points, val);
            }
            else
            {
                points = FoldX(points, val);
            }
        }

        return points.Count.ToString();
    }

    public override string SolveB(string input)
    {
        var (points, folds) = Parse(input);
        foreach (var (y, val) in folds)
        {
            if (y)
            {
                points = FoldY(points, val);
            }
            else
            {
                points = FoldX(points, val);
            }
        }
        var s = Format(points);
        Console.WriteLine(s);
        return "B";
    }

    public Day13()
    {
        Tests = new()
        {
            new("A", @"6,10
0,14
9,10
0,3
10,4
4,11
6,0
6,12
4,1
0,13
10,12
3,4
3,0
8,4
1,10
2,14
8,10
9,0

fold along y=7
fold along x=5", "17", SolveA),
            new("B", @"6,10
0,14
9,10
0,3
10,4
4,11
6,0
6,12
4,1
0,13
10,12
3,4
3,0
8,4
1,10
2,14
8,10
9,0

fold along y=7
fold along x=5", "B", SolveB)
        };
    }
}