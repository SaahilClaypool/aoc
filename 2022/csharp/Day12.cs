namespace Aoc2022;


public partial class Day12 : Day
{
    record ElevationMap(
        List<List<char>> Grid,
        Pos Start,
        Pos End,
        Pos Current,
        int Steps
    )
    {
        public Dictionary<Pos, int> VisitedEstimatedCost { get; set; } = new();
        public char this[Pos index] => Grid[index.Row][index.Col];

        public IEnumerable<Pos> ValidMoves() =>
            Surrounding(Current).Where(p => this[Current] >= this[p] - 1 &&
                // viable if haven't explored
                // or the minimum cost from there to end is < current explored cost
                (!VisitedEstimatedCost.ContainsKey(p) || VisitedEstimatedCost[p] > MinCost(p)));

        public IEnumerable<Pos> Surrounding(Pos current)
        {
            for (var r = current.Row - 1; r <= current.Row + 1; r++)
                for (var c = current.Col - 1; c <= current.Col + 1; c++)
                    if ((r == current.Row || c == current.Col) &&
                        !(r == current.Row && c == current.Col) &&
                        c >= 0 && c < Grid[0].Count && r >= 0 && r < Grid.Count)
                        yield return new(r, c);
        }

        public int Heuristic() => MinCost(Current);
        
        public int MinCost(Pos p) =>
            Steps +
            Math.Abs(p.Row - p.Row) +
            Math.Abs(p.Col - p.Col);

        public ElevationMap TakeMove(Pos p)
        {
            VisitedEstimatedCost[p] = MinCost(p);
            return this with { Current = p, Steps = Steps + 1 };
        }
    };

    static int AStar(ElevationMap map)
    {
        var moves = new PriorityQueue<ElevationMap, int>();
        Log($"Start {map.Start} End {map.End}");
        moves.Enqueue(map, map.Heuristic());
        while (moves.TryDequeue(out var currentMove, out var priority))
        {
            Log($"Current move: {currentMove.Current} cost {currentMove.Heuristic()}");
            if (currentMove.Current == currentMove.End)
            {
                return currentMove.Steps;
            }
            foreach (var move in currentMove.ValidMoves().Select(currentMove.TakeMove))
            {
                Log($"From {currentMove.Current} to {move.Current} ({move.Heuristic()})");
                moves.Enqueue(move, move.Heuristic());
            }
        }
        return int.MaxValue;
    }

    public override string SolveA(string input)
    {
        var start = Parse(input);
        var bestSteps = AStar(start);
        return bestSteps.ToString();
    }

    public override string SolveB(string input)
    {
        var start = Parse(input);
        var locationAs = start.Grid.SelectMany((row, r) =>
            row.Select((col, c) => (r, c, Letter: col)))
            .Where(tuple => tuple.Letter == 'a')
            .Select(tuple => new Pos(tuple.r, tuple.c))
            .ToList();
        
        foreach (var loc in locationAs)
        {
            Log(loc);
        }

        var costs = locationAs.Select(startingLoc =>
        {
            return (startingLoc, Cost: AStar(start with { Start = startingLoc, Current = startingLoc, VisitedEstimatedCost = new() }));
        }
        ).ToList();
        return costs.MinBy(m => m.Cost).Cost.ToString();
    }

    public Day12()
    {
        Tests = new()
        {
            new("A",
            """
            Sabqponm
            abcryxxl
            accszExk
            acctuvwj
            abdefghi
            """,
            "31",
            SolveA),
            new("B",
            """
            Sabqponm
            abcryxxl
            accszExk
            acctuvwj
            abdefghi
            """,
            "29",
            SolveB)
        };
    }

    static ElevationMap Parse(string input)
    {
        var grid = new List<List<char>>();
        Pos start = new(0, 0);
        Pos end = new(0, 0);
        foreach (var (line, row) in input.Lines().WithIndex())
        {
            grid.Add(new());
            foreach (var (c, col) in line.WithIndex())
            {
                if (c == 'S')
                {
                    start = new(row, col);
                    grid[row].Add('a');
                }
                else if (c == 'E')
                {
                    end = new(row, col);
                    grid[row].Add('z');
                }
                else
                {
                    grid[row].Add(c);
                }
            }
        }
        return new(grid, start, end, start, 0);
    }
    record Pos(int Row, int Col);
}