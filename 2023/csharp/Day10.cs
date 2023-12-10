namespace Aoc.Solutions.Y2023;

public class Day10 : Day
{
    Dictionary<Pos, char> Parse(string input) =>
        input
            .Lines()
            .SelectMany((line, row) =>
                line.Select((c, col) => (new Pos(row, col), c))
            ).ToDictionary();

    record Pos(int Row, int Col)
    {
        public override string ToString()
        {
            return $"({Row}, {Col})";
        }

        public Pos Add(Pos other) => new(Row + other.Row, Col + other.Col);
        public IEnumerable<Pos> Surrounding() => new[] {
            new Pos(-1, -1), new Pos(-1, 0), new Pos(-1, 1),
            new Pos(0, 1),
            new Pos(1, 1), new Pos(1, 0), new Pos(1, -1),
            new Pos(0, -1)
        }.Select(Add);
    }

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
            'S' => [new(-1, 0), new(0, 1), new(1, 0), new(0, -1)],
            _ => throw new NotImplementedException($"Invalid input {s}")
        };

    (List<Pos> Left, List<Pos> Right) Partition(char c, Pos prev, Pos cur)
    {
        var adj = Adj(c).Select(cur.Add);
        var fromTop = cur.Row > prev.Row;
        var fromLeft = cur.Col > prev.Col;
        var fromRight = cur.Col < prev.Col;
        var fromBot = cur.Row < prev.Row;
        var left = new List<Pos>();
        var right = new List<Pos>();
        // top left corner is right or left
        var onLeft = c switch
        {
            '|' when fromTop => false,
            '-' when fromRight => false,
            'L' when fromTop => false,
            'J' when fromTop => false,
            '7' when fromBot => false,
            'F' when fromRight => false,
            _ => true
        };
        var startOnLeft = onLeft;
        // starting from top left, until we hit the exit of the prev node
        // add to the left side
        foreach (var s in cur.Surrounding())
        {
            if (adj.Contains(s)) { onLeft = !onLeft; continue; }
            if (onLeft) left.Add(s);
            else right.Add(s);
        }
        return (left, right);
    }

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

    void PrintGrid(string input, HashSet<Pos> left, HashSet<Pos> right, HashSet<Pos> loop)
    {
        if (!IsTest) return;

        var cols = input.Lines().First().Length;

        foreach (var i in Enumerable.Range(-1, cols + 1))
        {
            Console.Write($"{i,3} ");
        }
        Console.WriteLine();
        foreach (var (line, row) in input.Lines().WithIndex())
        {
            Console.Write($"{row,3}:");
            foreach (var (c, col) in line.WithIndex())
            {
                var pos = new Pos(row, col);
                var x = left.Contains(pos) ? 'I'
                    : right.Contains(pos) ? '0'
                    : loop.Count == 0 || loop.Contains(pos) ? c
                    : ' ';
                Console.Write($"  {x} ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    // find the cycle (need the number of points until cycle starts + length of cycle)
    // then can count all the places that we will have wins
    // fold the wins together to find the fist win
    public override string SolveB(string input)
    {
        PrintGrid(input, new(), new(), new());
        var pipes = Parse(input);
        var (start, _) = pipes.Where(kvp => kvp.Value == 'S').First();
        var possible = new[] { '|', '-', 'L', 'J', '7', 'F', '.', 'S' };
        var loop = possible.Choose(x =>
        {
            pipes[start] = x;
            var path = WalkLoop(pipes, start);
            return path;
        }).First();

        // one of these must be interior
        var (left, right) = (new HashSet<Pos>(), new HashSet<Pos>());

        var prev = start;
        var empty = (Pos p) => !loop.Contains(p) && pipes.ContainsKey(p);
        foreach (var p in loop.Skip(1))
        {
            var leftToRight = p.Col > prev.Col || p.Row < prev.Row;
            var sym = pipes[p];
            var (leftPartition, rightPartition) = Partition(sym, prev, p);
            left.AddAll(leftPartition.Where(empty));
            right.AddAll(rightPartition.Where(empty));

            prev = p;
        }

        foreach (var l in left.ToList()) Expand(left, l);
        foreach (var r in right.ToList()) Expand(right, r);

        PrintGrid(input, left, right, loop.ToHashSet());

        return (left.Count < right.Count ? left.Count : right.Count).ToString();
        // with two partitions, label one as exterior / interior
        void Expand(HashSet<Pos> pos, Pos p)
        {
            foreach (var s in p.Surrounding())
            {
                if (empty!(s) && !pos.Contains(s))
                {
                    pos.Add(s);
                    Expand(pos, s);
                }
            }
        }

    }

    public Day10()
    {
        Tests = new()
        {
            new("a", SampleA, "4", SolveA),
            new("a2", SampleA2, "8", SolveA),
            new("b", SampleA, "1", SolveB),
            new("b1", SampleB1, "4", SolveB),
            new("b2", SampleB2, "8", SolveB),
            new("b3", SampleB3, "10", SolveB)
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

    static string SampleB1 = """
    ...........
    .S-------7.
    .|F-----7|.
    .||.....||.
    .||.....||.
    .|L-7.F-J|.
    .|..|.|..|.
    .L--J.L--J.
    ...........
    """;

    static string SampleB2 = """
    .F----7F7F7F7F-7....
    .|F--7||||||||FJ....
    .||.FJ||||||||L7....
    FJL7L7LJLJ||LJ.L-7..
    L--J.L7...LJS7F-7L7.
    ....F-J..F7FJ|L7L7L7
    ....L7.F7||L7|.L7L7|
    .....|FJLJ|FJ|F7|.LJ
    ....FJL-7.||.||||...
    ....L---J.LJ.LJLJ...
    """;

    static string SampleB3 = """
    FF7FSF7F7F7F7F7F---7
    L|LJ||||||||||||F--J
    FL-7LJLJ||||||LJL-77
    F--JF--7||LJLJ7F7FJ-
    L---JF-JLJ.||-FJLJJ7
    |F|F-JF---7F7-L7L|7|
    |FFJF7L7F-JF7|JL---7
    7-L-JL7||F7|L7F-7F7|
    L.L7LFJ|||||FJL7||LJ
    L7JLJL-JLJLJL--JLJ.L
    """;
}