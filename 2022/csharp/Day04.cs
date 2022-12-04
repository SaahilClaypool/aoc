namespace Aoc2022;


public class Day04 : Day
{
    record ERange(long Left, long Right);
    IEnumerable<(ERange left, ERange right)> Parse(string input) =>
        input.Split('\n')
            .Select(line =>
                line
                    .Split(',')
                    .Select(part =>
                    {
                        var parts = part.Split('-').Select(int.Parse).ToList();
                        return new ERange(parts[0], parts[1]);
                    }))
            .Select(parsed => (parsed.First(), parsed.Skip(1).First()));

    public override string SolveA(string input)
    {
        var contained = ((ERange left, ERange right) range) =>
        {
            var (left, right) = range;
            return
                left.Left <= right.Left && left.Right >= right.Right ||
                right.Left <= left.Left && right.Right >= left.Right;
        };

        return
             Parse(input)
            .Where(contained)
            .Count()
            .ToString();

    }

    public override string SolveB(string input)
    {
        var overlaps = ((ERange left, ERange right) range) =>
        {
            var (left, right) = range;
            return
                left.Left <= right.Left && left.Right >= right.Left ||
                right.Left <= left.Left && right.Right >= left.Left ||
                left.Left <= right.Right && left.Right >= right.Right ||
                right.Left <= left.Right && right.Right >= left.Right;
        };

        return
             Parse(input)
            .Where(overlaps)
            .Count()
            .ToString();

    }

    public Day04()
    {
        Tests = new()
        {
            new("A",
            """
            2-4,6-8
            2-3,4-5
            5-7,7-9
            2-8,3-7
            6-6,4-6
            2-6,4-8
            """,
            "2",
            SolveA),
            new("B",
            """
            2-4,6-8
            2-3,4-5
            5-7,7-9
            2-8,3-7
            6-6,4-6
            2-6,4-8
            """,
            "4", 
            SolveB)
        };
    }
}