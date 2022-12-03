using Aoc.Solutions.Day11;

namespace Aoc.Solutions.Day15;

public class Day15 : Day
{
    static NoDiagGrid<int> Parse(string input) =>
        input
        .Split('\n')
        .Select(line => line.Select(Fn.CharToInt).ToList())
        .ToList()
        .Then(d => new NoDiagGrid<int>(d));

    public static int Djikstras(NoDiagGrid<int> grid)
    {
        var bottomRight = (grid.Rows - 1, grid.Cols - 1);
        Log($"Bottom right = {bottomRight}");
        Dictionary<(int r, int c), (int, (int r, int c) from)> costMatrix = new();
        PriorityQueue<(int r, int c), int> pq = new();
        pq.Enqueue((0, 0), 0);
        while (pq.TryDequeue(out var v, out var cost))
        {
            Log($"v = {v} c = {cost}");
            foreach (var next in grid.Surrounding(v))
            {
                if (costMatrix.ContainsKey(next))
                {
                    continue;
                }
                var nodeCost = grid[next] + cost;
                if (next == bottomRight)
                {
                    return nodeCost;
                }
                Log($"next = {next} c = {nodeCost}");
                costMatrix[next] = (nodeCost, v);
                pq.Enqueue(next, nodeCost);
            }
        }
        throw new Exception("Should have been found");
    }

    public static NoDiagGrid<int> CopyGrid5Times(NoDiagGrid<int> grid)
    {
        List<List<int>> superGrid =
            Range(0, grid.Rows * 5)
            .Select(r => Range(0, grid.Cols * 5).ToList())
            .ToList();

        foreach (var r in Range(0, 5))
        {
            foreach (var c in Range(0, 5))
            {
                foreach (var ri in Range(0, grid.Rows))
                {
                    foreach (var ci in Range(0, grid.Cols))
                    {
                        var newVal = Wrap(grid[(ri, ci)] + r + c);
                        Log($"{r} {c} {(ri, ci)} {grid[(ri, ci)]} -> {newVal}");
                        superGrid[r * grid.Rows + ri][c * grid.Cols + ci] = newVal;
                    }
                }
            }
        }
        return new(superGrid);
    }

    private static int Wrap(int v) => v >= 10 ? v % 10 + 1 : v;

    public override string SolveA(string input)
    {
        var grid = Parse(input);
        var cost = Djikstras(grid);
        return cost.ToString();
    }

    public override string SolveB(string input)
    {
        var grid = Parse(input);
        var superGrid = CopyGrid5Times(grid);
        Log(superGrid.ToString());
        var cost = Djikstras(superGrid);
        return cost.ToString();
    }

    public Day15()
    {
        Tests = new()
        {
//             new("A", @"1163751742
// 1381373672
// 2136511328
// 3694931569
// 7463417111
// 1319128137
// 1359912421
// 3125421639
// 1293138521
// 2311944581", "40", SolveA),
            new("B", @"1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581", "315", SolveB)
        };
    }
}