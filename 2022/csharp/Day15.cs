namespace Aoc2022;


public partial class Day15 : Day
{
    public override string SolveA(string input)
    {
        return SolveAAt(input, 2000000);
    }

    int OverlappingOnRow(Sensor s, Pos p, int minX, int maxX)
    {
        // because manhattan distance, its range - y distance to line * 2
        var yDistanceToLine = Math.Abs(s.Loc.Y - p.Y);
        var reducedRange = int.Max(0, s.Range - yDistanceToLine);
        var fullLineCoverage = reducedRange * 2;

        var left = s.Loc.X - reducedRange;
        var right = s.Loc.X + reducedRange;
        var offLeft = left < minX ? minX - left : 0;
        var offRight = right > maxX ? right - maxX : 0;

        return fullLineCoverage - offLeft - offRight;
    }

    string SolveAAt(string input, int y)
    {
        var sensors = Parse(input).OrderByDescending(r => r.Range).ToList();
        var minX = sensors.Min(s => s.Loc.X - s.Range);
        var maxX = sensors.Max(s => s.Loc.X + s.Range);

        var beacons = sensors.Select(s => s.Beacon).ToHashSet();

        return Enumerable
            .Range(minX - 1, maxX - minX + 2)
            .Select(p => new Pos(p, y))
            .Where(p =>
                !beacons.Contains(p) &&
                sensors.Any(s => s.InRange(p)))
            .Count()
            .ToString();
    }

    public override string SolveB(string input)
    {
        return SolveBBounded(input, 4000000);
    }

    public override string SolveBBounded(string input, int max)
    {
        throw new  NotImplementedException();
    }

    public Day15()
    {
        Tests = new()
        {
            new("A",
            """
            Sensor at x=2, y=18: closest beacon is at x=-2, y=15
            Sensor at x=9, y=16: closest beacon is at x=10, y=16
            Sensor at x=13, y=2: closest beacon is at x=15, y=3
            Sensor at x=12, y=14: closest beacon is at x=10, y=16
            Sensor at x=10, y=20: closest beacon is at x=10, y=16
            Sensor at x=14, y=17: closest beacon is at x=10, y=16
            Sensor at x=8, y=7: closest beacon is at x=2, y=10
            Sensor at x=2, y=0: closest beacon is at x=2, y=10
            Sensor at x=0, y=11: closest beacon is at x=2, y=10
            Sensor at x=20, y=14: closest beacon is at x=25, y=17
            Sensor at x=17, y=20: closest beacon is at x=21, y=22
            Sensor at x=16, y=7: closest beacon is at x=15, y=3
            Sensor at x=14, y=3: closest beacon is at x=15, y=3
            Sensor at x=20, y=1: closest beacon is at x=15, y=3
            """,
            "26",
            input => SolveAAt(input, 10)),
            new("B",
            """
            Sensor at x=2, y=18: closest beacon is at x=-2, y=15
            Sensor at x=9, y=16: closest beacon is at x=10, y=16
            Sensor at x=13, y=2: closest beacon is at x=15, y=3
            Sensor at x=12, y=14: closest beacon is at x=10, y=16
            Sensor at x=10, y=20: closest beacon is at x=10, y=16
            Sensor at x=14, y=17: closest beacon is at x=10, y=16
            Sensor at x=8, y=7: closest beacon is at x=2, y=10
            Sensor at x=2, y=0: closest beacon is at x=2, y=10
            Sensor at x=0, y=11: closest beacon is at x=2, y=10
            Sensor at x=20, y=14: closest beacon is at x=25, y=17
            Sensor at x=17, y=20: closest beacon is at x=21, y=22
            Sensor at x=16, y=7: closest beacon is at x=15, y=3
            Sensor at x=14, y=3: closest beacon is at x=15, y=3
            Sensor at x=20, y=1: closest beacon is at x=15, y=3
            """,
            "56000011",
            input => SolveBBounded(input, 20))
        };
    }

    List<Sensor> Parse(string input)
    {
        var re = new Regex(@"x=(-?\d+), y=(-?\d+)");
        return input
            .Lines()
            .Select(line =>
            {
                var positions = re.Matches(line)
                    .Select(m =>
                        new Pos(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)))
                    .ToList();
                return new Sensor(positions[0], positions[1]);
            }).ToList();
    }

    record Sensor(Pos Loc, Pos Beacon)
    {
        public int Range => Loc.Distance(Beacon);
        public bool InRange(Pos p) => Loc.Distance(p) <= Range;
    };
    record Pos(int X, int Y)
    {
        public int Distance(Pos other) =>
            int.Abs(X - other.X) + int.Abs(Y - other.Y);
    };
}