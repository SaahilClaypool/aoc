namespace Aoc2022;


public partial class Day08 : Day
{
    static IEnumerable<int> Row(List<List<int>> grid, int idx) => grid[idx];
    static IEnumerable<int> Col(List<List<int>> grid, int idx) => grid.Select(row => row[idx]).ToList();

    static IEnumerable<int> Up(List<List<int>> grid, int r, int c)
    {
        for (var ri = r - 1; ri >= 0; ri--)
        {
            yield return grid[ri][c];
        }
    }
    static IEnumerable<int> Down(List<List<int>> grid, int r, int c)
    {
        for (var ri = r + 1; ri < grid.Count; ri++)
        {
            yield return grid[ri][c];
        }
    }
    static IEnumerable<int> Left(List<List<int>> grid, int r, int c)
    {
        for (var ci = c - 1; ci >= 0; ci--)
        {
            yield return grid[r][ci];
        }
    }
    static IEnumerable<int> Right(List<List<int>> grid, int r, int c)
    {
        for (var ci = c + 1; ci < grid[0].Count; ci++)
        {
            yield return grid[r][ci];
        }
    }

    static List<List<int>> Parse(string input) =>
        input.Split("\n")
            .Select(line => line
                .Select(c => int.Parse($"{c}"))
                .ToList())
        .ToList();


    public override string SolveA(string input)
    {
        var grid = Parse(input);
        var visibleGrid = new List<List<bool>>();
        foreach (var r in Enumerable.Range(0, grid.Count))
        {
            var row = Row(grid, r);
            visibleGrid.Add(Visible(row.ToList()));
        }
        // look from the right
        foreach (var r in Enumerable.Range(0, grid.Count))
        {
            var row = Row(grid, r).Reverse().ToList();
            var visible = Visible(row).Reverse<bool>();
            foreach (var (i, c) in visible.WithIndex())
            {
                visibleGrid[r][c] = i || visibleGrid[r][c];
            }
        }

        // look from the top
        foreach (var c in Enumerable.Range(0, grid[0].Count))
        {
            var col = Col(grid, c).ToList();
            var visible = Visible(col);
            foreach (var (i, r) in visible.WithIndex())
            {
                visibleGrid[r][c] = i || visibleGrid[r][c];
            }
        }

        // look from the bottom
        foreach (var c in Enumerable.Range(0, grid[0].Count))
        {
            var col = Col(grid, c).Reverse().ToList();
            var visible = Visible(col).Reverse<bool>();
            foreach (var (i, r) in visible.WithIndex())
            {
                visibleGrid[r][c] = i || visibleGrid[r][c];
            }
        }

        return visibleGrid.SelectMany(v => v).Where(v => v).Count().ToString();
    }

    static List<bool> Visible(List<int> vec)
    {
        int curMax = vec[0];
        var maxes = new List<int>() { -1 };
        foreach (var (val, idx) in vec.WithIndex())
        {
            maxes.Add(int.Max(val, maxes[idx]));
        }
        return vec.Zip(maxes).Select(pair => pair.First > pair.Second).ToList();
    }

    public override string SolveB(string input)
    {
        var grid = Parse(input);
        var scoreGrid = new List<List<long>>();
        foreach (var r in Enumerable.Range(0, grid.Count))
        {
            scoreGrid.Add(new());
            foreach (var c in Enumerable.Range(0, grid[0].Count))
            {
                scoreGrid[r].Add(Score(grid, r, c));
            }
        }
        return scoreGrid.SelectMany(g => g).Max().ToString();
    }

    private long Score(List<List<int>> grid, int r, int c)
    {
        var height = grid[r][c];
        Log("UP");
        var up = Score(Up(grid, r, c), height);
        Log("LEFT");
        var left = Score(Left(grid, r, c), height);
        Log("RIGHT");
        var right = Score(Right(grid, r, c), height);
        Log("DOWN");
        var down = Score(Down(grid, r, c), height);

        var score = up * left * right * down;
        Log($"[{r}][{c}] = {grid[r][c]} : ({up}, {left}, {down}, {right}) = {score}");

        return score;
    }

    private static long Score(IEnumerable<int> nums, int max)
    {
        var seen = 0;
        var largest = 0;
        foreach (var i in nums)
        {
            Log(i);
            seen++;
            if (i >= max)
            {
                break;
            }
            largest = int.Max(largest, i);
        }
        return seen;
    }

    public Day08()
    {
        Tests = new()
        {
            new("A",
                """
                30373
                25512
                65332
                33549
                35390
                """,
                "21",
                SolveA
            ),
            new("B",
                """
                30373
                25512
                65332
                33549
                35390
                """,
                "8",
                SolveB
            ),
            new("B1",
                """
                30373
                25512
                65332
                33549
                35390
                """,
                "4",
                i => Score(Parse(i), 1, 2).ToString()
            ),
            new("B2",
                """
                30373
                25512
                65332
                33549
                35390
                """,
                "8",
                i => Score(Parse(i), 3, 2).ToString()
            )
        };
    }
}