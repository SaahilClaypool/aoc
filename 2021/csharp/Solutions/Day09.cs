global using static Aoc.Runner.LogHelpers;
namespace Aoc.Solutions.Day09;

public class Map
{
    public Map(List<List<int>> heights) => Heights = heights;

    public List<List<int>> Heights { get; }
    public int Rows => Heights.Count;
    public int Cols => Heights[0].Count;

    public IEnumerable<(int r, int c)> SurroundingPoints(int r, int c)
    {
        foreach (var ri in Range(r - 1, 3))
            foreach (var ci in Range(c - 1, 3))
                if (ri >= 0 && ri < Rows && ci >= 0 && ci < Cols && (ci == c || ri == r) && !(ri == r && ci == c))
                    yield return (ri, ci);
    }

    public IEnumerable<int> Surrounding(int r, int c) => SurroundingPoints(r, c).Select(p => Heights[p.r][p.c]);

    public bool IsLow(int r, int c) =>
        Surrounding(r, c).All(h => h > Heights[r][c]);

    public int Risk(int r, int c) =>
        IsLow(r, c) ? Heights[r][c] + 1 : 0;

    public IEnumerable<(int r, int c)> Points()
    {
        foreach (var ri in Range(0, Rows))
            foreach (var ci in Range(0, Cols))
                yield return (ri, ci);
    }

    // get basin from starting low point
    public List<(int r, int c)> GetBasin(int r, int c)
    {
        Log($"\nLow point: {(r, c)}");
        var basin = new HashSet<(int r, int c)>() { (r, c) };
        var inBasin = (int ri, int ci) =>
            SurroundingPoints(ri, ci).Any(p => basin.Contains((p.r, p.c)));

        var ring = 1;
        var newMatches = true;
        while (newMatches)
        {
            Log($"Ring = {ring} - lastMatch = {newMatches}");
            newMatches = false;
            foreach (var ri in Range(r - ring, ring * 2 + 1))
            {
                foreach (var ci in Range(c - ring, ring * 2 + 1))
                {
                    if (ri >= 0 && ri < Rows && ci >= 0 && ci < Cols && Heights[ri][ci] != 9)
                    {
                        var anySurrounding = inBasin(ri, ci);
                        Log($"Checking {(ri, ci) } = {Heights[ri][ci]} - {anySurrounding }");
                        if (anySurrounding && !basin.Contains((ri, ci)))
                        {
                            Log($"\tAdding {(ri, ci)}");
                            basin.Add((ri, ci));
                            newMatches = true;
                        }
                    }
                }
            }
            ring += 1;
        }
        Log($"Basin from {(r, c)} - {basin.Select(b => b.ToString()).Join(",")}");
        return basin.ToList();
    }
}

public class Day09 : Day
{
    public static Map Parse(string inputString) =>
        inputString
            .Split('\n')
            .Select(line => line.Trim().Select(c => int.Parse(new string(new char[] { c }))).ToList())
            .ToList()
            .Then(lines => new Map(lines));

    public override string SolveA(string input)
    {
        var map = Parse(input);
        return map.Points().Select(point => map.Risk(point.r, point.c)).Sum().ToString();
    }

    public override string SolveB(string input)
    {
        var map = Parse(input);
        var lowPoints = map.Points().Where(p => map.IsLow(p.r, p.c)).ToList();
        var basins =
            lowPoints
            .Select(p => map.GetBasin(p.r, p.c))
            .ToList();
        return
            basins
            .Select(b => b.Count)
            .OrderByDescending(c => c)
            .Take(3)
            .Aggregate(1, (i, state) => i * state)
            .ToString();
    }

    public Day09()
    {
        Tests = new()
        {
            new("A", @"2199943210
3987894921
9856789892
8767896789
9899965678", "15", SolveA),
            new("B", @"2199943210
3987894921
9856789892
8767896789
9899965678", "1134", SolveB)
        };
    }
}