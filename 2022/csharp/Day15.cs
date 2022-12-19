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
    static (long start, long end)? SensorRowCoverage(Sensor s, long y)
    {
        var yDist = Math.Abs(s.Loc.Y - y);
        var reducedRange = s.Range - yDist;
        if (reducedRange < 0)
            return null;

        var xStart = s.Loc.X - reducedRange;
        var xEnd = s.Loc.X + reducedRange;
        return (xStart, xEnd);
    }

    static IEnumerable<(long start, long end)> MergeRanges(IEnumerable<(long start, long end)> ranges)
    {
        long? left = null;
        long? right = null;
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

    string SolveAAt(string input, long y)
    {
        var sensors = Parse(input).OrderByDescending(r => r.Range).ToList();
        var beacons = sensors.Select(s => s.Beacon).ToHashSet();

        var mergedRanges = CoverageOnRow(y, sensors);
        var beaconsOnRow = beacons.Where(b => b.Y == y).Count();
        var rangeCoverge = mergedRanges
            .Select(r => r.end - r.start + 1)
            .Sum();

        return (rangeCoverge - beaconsOnRow).ToString();
    }

    private static IEnumerable<(long start, long end)> CoverageOnRow(long y, List<Sensor> sensors)
    {
        var coverageRanges = sensors
            .Select(s => SensorRowCoverage(s, y))
            .Where(r => r != null)
            .Select(r => r!.Value)
            .OrderBy(r => r.start)
            .ToList();

        var mergedRanges = MergeRanges(coverageRanges);
        return mergedRanges;
    }

    public override string SolveB(string input)
    {
        return SolveBBounded(input, 4000000);
    }

    public string SolveBBounded(string input, long max)
    {
        var sensors = Parse(input).OrderByDescending(r => r.Range).ToList();
        var beacons = sensors.Select(s => s.Beacon).ToHashSet();

        Pos p = default!;

        /// <summary>
        /// This loops through all points - we only need to loop through points on the perimeter of the diamond
        /// </summary>
        for (var y = 0; y <= max; y++)
        {
            var coverage = CoverageOnRow(y, sensors)
                .Select(tuple => (
                    start: Math.Clamp(tuple.start, 0, max),
                    end: Math.Clamp(tuple.end, 0, max)
                ))
                .ToList();
            
            foreach (var (A, B) in coverage.Pairs())
            {
                if (B.start - A.end == 2)
                {
                    p = new(A.end + 1, y);
                    goto outer;
                }
            }
        }
        outer: ;

        return (p.X * 4000000 + p.Y).ToString();
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
                        new Pos(long.Parse(m.Groups[1].Value), long.Parse(m.Groups[2].Value)))
                    .ToList();
                return new Sensor(positions[0], positions[1]);
            }).ToList();
    }

    record Sensor(Pos Loc, Pos Beacon)
    {
        public long Range => Loc.Distance(Beacon);
        public bool InRange(Pos p) => Loc.Distance(p) <= Range;
    };
    record Pos(long X, long Y)
    {
        public long Distance(Pos other) =>
            long.Abs(X - other.X) + long.Abs(Y - other.Y);
    };
}