namespace Aoc2022;


public partial class Day18 : Day
{
    record Pos(int X, int Y, int Z)
    {
        public IEnumerable<Pos> Adjacent()
        {
            foreach (var x in Enumerable.Range(X - 1, 3))
                foreach (var y in Enumerable.Range(Y - 1, 3))
                    foreach (var z in Enumerable.Range(Z - 1, 3))
                        if (!(x == X && y == Y && z == Z) &&
                            (x == X && y == Y ||
                                x == X && z == Z ||
                                y == Y && z == Z))
                            yield return new(x, y, z);

        }
    }

    List<Pos> Parse(string input) =>
        input.Lines()
            .Select(line => line.Split(",").Select(int.Parse).ToArray())
            .Select(p => new Pos(p[0], p[1], p[2]))
        .ToList();

    public override string SolveA(string input)
    {
        var cubes = Parse(input).ToHashSet();
        var sides = cubes.SelectMany(c => c.Adjacent()).ToList();
        Log($"total sides: {sides.Count}");
        var exposed = sides.Where(s => !cubes.Contains(s));
        return exposed.Count().ToString();
    }

    public override string SolveB(string input)
    {
        var cubes = Parse(input).ToHashSet();
        var sides = cubes.SelectMany(c => c.Adjacent()).ToList();
        var exposed = sides.Where(s => !cubes.Contains(s)).ToList();
        var exposedSet = exposed.ToHashSet();
        var minX = exposedSet.Select(s => s.X).Min() - 1;
        var maxX = exposedSet.Select(s => s.X).Max() + 1;
        var minY = exposedSet.Select(s => s.Y).Min() - 1;
        var maxY = exposedSet.Select(s => s.Y).Max() + 1;
        var minZ = exposedSet.Select(s => s.Z).Min() - 1;
        var maxZ = exposedSet.Select(s => s.Z).Max() + 1;

        // choose a point
        // expand until can't expand anymore
        // if touches wall -> entire group "exterior"

        var setOfSets = new HashSet<Pos>();
        var interior = new HashSet<Pos>();

        while (true)
        {
            var seed = exposedSet.Except(setOfSets).FirstOrDefault();
            if (seed is null)
            {
                break;
            }
            var changed = true;
            HashSet<Pos> currentSet = new()
            {
                seed
            };
            var exterior = false;
            while (changed)
            {
                changed = false;
                var pastCount = currentSet.Count;
                // could ignore already processed points
                foreach (var point in currentSet.ToList())
                {
                    if (
                        point.X < minX || point.X > maxX ||
                        point.Y < minY || point.Y > maxY ||
                        point.Z < minZ || point.Z > maxZ
                    )
                    {
                        exterior = true;
                        continue;
                    }
                    var reachable = point.Adjacent().Except(cubes).ToHashSet();
                    if (reachable.Contains(new(2, 2, 5)))
                    {
                        
                    }
                    currentSet.AddAll(reachable);
                    if (currentSet.Count != pastCount)
                    {
                        changed = true;
                    }
                }
            }
            // while true find all adjacent non occupied points
            // if touches exterior limits -> entire set is exterior
            // else interior.
            // return all edges - interior
            setOfSets.AddAll(currentSet);
            if (!exterior)
            {
                interior.AddAll(currentSet);
            }
        }
        return exposed.Where(s => !interior.Contains(s)).Count().ToString();
    }

    public Day18()
    {
        Tests = new()
        {
            new("Sample",
            """
            1,1,1
            2,1,1
            """,
            "10",
            SolveA),
            new("A",
            """
            2,2,2
            1,2,2
            3,2,2
            2,1,2
            2,3,2
            2,2,1
            2,2,3
            2,2,4
            2,2,6
            1,2,5
            3,2,5
            2,1,5
            2,3,5
            """,
            "64",
            SolveA),
            new("B",
            """
            2,2,2
            1,2,2
            3,2,2
            2,1,2
            2,3,2
            2,2,1
            2,2,3
            2,2,4
            2,2,6
            1,2,5
            3,2,5
            2,1,5
            2,3,5
            """,
            "58",
            SolveB)
        };
    }
}