namespace Aoc.Solutions.Day11;


class OctoGrid : Grid<int>
{
    public OctoGrid(List<List<int>> nums) : base(nums)
    {
    }
    
    public int Step()
    {
        // inc
        foreach (var (r, c) in All())
        {
            State[r][c] += 1;
        }
        // burst
        var hasBurst = new HashSet<(int r, int c)>();
        var willBurst = All().Where(p => State[p.r][p.c] > 9 && !hasBurst.Contains(p)).ToList();
        while (willBurst.Any())
        {
            foreach (var burst in willBurst)
            {
                foreach (var p in Surrounding(burst))
                {
                    State[p.r][p.c] += 1;
                }
                hasBurst.Add(burst);
            }
            willBurst = All().Where(p => State[p.r][p.c] > 9 && !hasBurst.Contains(p)).ToList();
        }
        // reset
        foreach (var (r, c) in hasBurst)
        {
            State[r][c] = 0;
        }

        return hasBurst.Count;
    }
}

public class Day11 : Day
{
    static int CharToInt(char c) => c - '0';

    static OctoGrid Parse(string input) =>
        input
        .Split('\n')
        .Select(line => line.Select(CharToInt).ToList())
        .ToList()
        .Then(grid => new OctoGrid(grid));

    public override string SolveA(string input)
    {
        var grid = Parse(input);
        var count = Range(0, 100).Select(_ => grid.Step()).Sum();
        return count.ToString();
    }

    public override string SolveB(string input)
    {
        var grid = Parse(input);
        var flashed = 0;
        var gen = 0;
        while(flashed != grid.All().Count())
        {
            flashed = grid.Step();
            gen += 1;
        }
        return gen.ToString();
    }

    public Day11()
    {
        Tests = new()
        {
            new("A", @"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526", "1656", SolveA),
            new("A", @"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526", "195", SolveB)
        };
    }
}