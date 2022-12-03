namespace Aoc.Solutions.Day07;

public class Day07 : Day
{
    static List<int> Parse(string input) =>
        input.Split(',').Select(int.Parse).OrderBy(v => v).ToList();

    public override string SolveA(string inputString)
    {
        var input =
            Parse(inputString)
            .GroupBy(v => v)
            .OrderBy(v => v.Key)
            .Select(v => (v.Key, Count: v.Count()))
            .ToList();

        var costs = input.Indexed().Select(i =>
        {
            var (pos, idx) = i;
            var cost = input.Select(v => Math.Abs(v.Key - pos.Key) * v.Count).Sum();
            return cost;
        }).ToList();
        return costs.Min().ToString();
    }

    public override string SolveB(string inputString)
    {
        var input =
            Parse(inputString);

        var inputGrouped =
            input
            .GroupBy(v => v)
            .OrderBy(v => v.Key)
            .Select(v => (v.Key, Count: v.Count()))
            .ToList();

        var costs = Range(input.Min(), input.Max() - input.Min())
            .Select(i =>
            {
                var cost = inputGrouped.Select(v => CostB(i, v.Key) * v.Count).Sum();
                return cost;
            }).ToList();
        return costs.Min().ToString();
    }

    private int CostB(int k, int p)
    {
        var dist = Math.Abs(k - p);
        var cost = NumSum(dist);
        return cost;
    }
    private int NumSum(int v)
    {
        if (v % 2 == 0)
        {
            return v / 2 * (v + 1);
        }
        else
        {
            return NumSum(v + 1) - (v + 1);
        }
    }

    public Day07()
    {
        Tests = new()
        {
            new("A", "16,1,2,0,4,2,7,1,2,14", "37", SolveA),
            new("NumSum100", "100", "5050", i => NumSum(int.Parse(i)).ToString()),
            new("NumSum99", "99", "4950", i => NumSum(int.Parse(i)).ToString()),
            new("NumSum5", "5", "15", i => NumSum(int.Parse(i)).ToString()),
            new("B", "16,1,2,0,4,2,7,1,2,14", "168", SolveB)
        };
    }
}