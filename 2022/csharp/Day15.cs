namespace Aoc2022;


public partial class Day15 : Day
{
    public override string SolveA(string input)
    {
        return SolveAAt(input, 2000000);
    }

    /// <summary>
    /// for a given row, 
    /// return the sensors coverage on this row
    /// inclusive
    /// </summary>
    static (int start, int end)? SensorRowCoverage(Sensor s, int y)
    {
        var yDist = Math.Abs(s.Loc.Y - y);
        var reducedRange = s.Range - yDist;
        if (reducedRange < 0)
            return null;

        var xStart = s.Loc.X - reducedRange;
        var xEnd = s.Loc.X + reducedRange;
        return (xStart, xEnd);
    }

    static IEnumerable<(int start, int end)> MergeRanges(IEnumerable<(int start, int end)> ranges)
    {
        int? left = null;
        int? right = null;
        foreach (var range in ranges)
        {
            if (left is null)
            {
                (left, right) = range;
                continue;
            }
            if (range.start <= right)
            {
                right = Math.Max(range.end, right!.Value);
            }
            else
            {
                yield return (left!.Value, right!.Value);
                (left, right) = range;
            }
        }
        yield return (left!.Value, right!.Value);
    }

    string SolveAAt(string input, int y)
    {
        var sensors = Parse(input).OrderByDescending(r => r.Range).ToList();
        var beacons = sensors.Select(s => s.Beacon).ToHashSet();

        var coverageRanges = sensors
            .Select(s => SensorRowCoverage(s, y))
            .Where(r => r != null)
            .Select(r => r!.Value)
            .OrderBy(r => r.start)
            .ToList();

        var mergedRanges = MergeRanges(coverageRanges);
        var beaconsOnRow = beacons.Where(b => b.Y == y).Count();
        var rangeCoverge = mergedRanges
            .Select(r => r.end - r.start + 1)
            .Sum();

        return (rangeCoverge - beaconsOnRow).ToString();
    }

    public override string SolveB(string input)
    {
        return SolveBBounded(input, 4000000);
    }

    public string SolveBBounded(string input, int max)
    {
        throw new NotImplementedException();
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