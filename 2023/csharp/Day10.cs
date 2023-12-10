namespace Aoc.Solutions.Y2023;

public class Day10 : Day
{
    Dictionary<Pos, char> Parse(string input) =>
        input
            .Lines()
            .SelectMany((line, row) =>
                line.Select((c, col) => (new Pos(row, col), c))
            ).ToDictionary();

    record Pos(int Row, int Col);

    IEnumerable<Pos> Adj(char s) =>
        s switch
        {
            '|' => [new(-1, 0), new(1, 0)],
            '-' => [new(0, -1), new(0, 1)],
            'L' => [new(-1, 0), new(0, 1)],
            'J' => [new(-1, 0), new(0, -1)],
            '7' => [new(1, 0), new(0, -1)],
            'F' => [new(1, 0), new(0, 1)],
            '.' => [],
            'S' => [new(-1, 0), new(0, 1), new(1, 0), new(-1, 0)],
            _ => throw new NotImplementedException($"Invalid input {s}")
        };

    IEnumerable<Pos> Adj(Dictionary<Pos, char> pipes, Pos p)
    {
        if (!pipes.TryGetValue(p, out var pipe) && pipe != '.')
            return [];
        var adj = Adj(pipe);
        return adj.Select(offset => new Pos(Row: p.Row + offset.Row, Col: p.Col + offset.Col));
    }

    // valid if adjacent are connecting pipes
    bool Valid(Dictionary<Pos, char> pipes, Pos p)
    {
        if (pipes.GetValueOrDefault(p) == '.') return false;
        var adj = Adj(pipes, p);
        return adj.All(pos =>
        {
            var connectingPipes = Adj(pipes, pos);
            return connectingPipes.Any(connecting =>
            {
                return p == connecting;
            });
        });
    }

    List<Pos>? WalkLoop(Dictionary<Pos, char> pipes, Pos start)
    {
        var cur = start;
        HashSet<Pos> visited = new()
        {
            start
        };
        List<Pos> path = new()
        {
            start
        };

        while (Valid(pipes, cur))
        {
            var adj = Adj(pipes, cur).ToList();
            // if we hit the cycle
            if (adj.Contains(start) && path.Count > 2)
            {
                return path;
            }
            cur = adj
                .Where(p =>
                    !visited.Contains(p) &&
                        Valid(pipes, p))
                .FirstOrDefault();
            if (cur is null) return null;
            visited.Add(cur);
            path.Add(cur);
        }
        return null;
    }

    public override string SolveA(string input)
    {
        var pipes = Parse(input);
        var (start, _) = pipes.Where(kvp => kvp.Value == 'S').First();
        var possible = new[] { '|', '-', 'L', 'J', '7', 'F', '.', 'S' };

        foreach (var possibleChar in possible)
        {
            pipes[start] = possibleChar;
            var loop = WalkLoop(pipes, start);
            if (loop is not null)
                return (loop.Count / 2).ToString();
        }
        return "no loop found";
    }

    // find the cycle (need the number of points until cycle starts + length of cycle)
    // then can count all the places that we will have wins
    // fold the wins together to find the fist win
    public override string SolveB(string input)
    {
        return "B";
    }

    public Day10()
    {
        Tests = new()
        {
            new("a", SampleA, "4", SolveA),
            new("a", SampleA2, "8", SolveA)
        };
    }

    static string SampleA = """
    .....
    .S-7.
    .|.|.
    .L-J.
    .....
    """;

    static string SampleA2 = """
    ..F7.
    .FJ|.
    SJ.L7
    |F--J
    LJ...
    """;
}