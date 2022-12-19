namespace Aoc2022.Day17Solution;

using System;
using Pos = Aoc2022.GridType.Pos;

class World
{
    public record State(long ShapeNumber, long WindNumber, long Height, long Round);
    long ShapeNumber = 0;
    long WindNumber = 0;
    public List<int> Movements { get; set; } = new();
    HashSet<Pos> Blocks { get; set; } = new();
    public int Height => Blocks.Any() ? Blocks.Select(p => p.Row).Max() : 0;

    public bool Occupied(Pos p) => Blocks.Contains(p) || p.Col < 0 || p.Col >= 7 || p.Row <= 0;
    public bool Free(Pos p) => !Occupied(p);

    List<State> States { get; set; } = new();

    class Shape
    {
        public HashSet<Pos> InitialPoints { get; set; } = new();
        public List<Pos> Move(int dx, int dy) => InitialPoints.Select(p => new Pos(p.Row + dy, p.Col + dx)).ToList();
    }

    public (State prevState, State loopState, List<State>)? DropShape()
    {
        var shape = Shapes[ShapeNumber++ % Shapes.Length];
        var (X, Y) = (2, Height + 4);
        var points = shape.Move(X, Y);
        var turn = 0;
        while (true)
        {
            turn++;
            if (turn % 2 == 1) // start with wind
            {
                var dir = Movements[(int)(WindNumber++ % Movements.Count)];
                var newX = X + dir;
                var newPoints = shape.Move(newX, Y);
                // hit nothing -> valid move
                if (newPoints.All(Free))
                {
                    points = newPoints;
                    X = newX;
                }
            }
            else
            {
                Y -= 1;
                var newPoints = shape.Move(X, Y);
                // hit something -> stop dropping
                if (newPoints.Any(Occupied))
                {
                    break;
                }
                else
                {
                    points = newPoints;
                }
            }
        }
        Add(points);

        var loopState = new State(ShapeNumber % Shapes.Length, WindNumber % Movements.Count, Height, Round: ShapeNumber);
        if (FullRow(Height))
        {
            Console.WriteLine($"Clear!");
            if (States.FirstOrDefault(s => s with { Height = Height, Round = ShapeNumber } == loopState) is var prevState && prevState != null)
            {
                Console.WriteLine($"Found loop to loop! {loopState}");
                return (prevState, loopState, States);
            }
        }
        States.Add(loopState);
        return null;
    }

    private bool FullRow(int h) =>
        Blocks.Where(b => b.Row == h).Count() == 7;

    void Add(IEnumerable<Pos> points)
    {
        foreach (var p in points)
        {
            Blocks.Add(p);
        }
    }

    static readonly Shape[] Shapes = new[]
    {
        // line
        new Shape() { InitialPoints = new() { new(0, 0), new(0, 1), new(0, 2), new(0, 3) } },
        // cross
        new Shape() { InitialPoints = new() { new(0, 1), new(1, 0), new(1, 1), new(1, 2), new(2, 1) } },
        // L
        new Shape() { InitialPoints = new() { new(2, 2), new(1, 2), new(0, 0), new(0, 1), new(0, 2) } },
        // |
        new Shape() { InitialPoints = new() { new(0, 0), new(1, 0), new(2, 0), new(3, 0) } },
        // box
        new Shape() { InitialPoints = new() { new(0, 0), new(0, 1), new(1, 1), new(1, 0) } }
    };
}

public partial class Day17 : Day
{
    static World Parse(string input) => new() { Movements = input.Select(c => c == '<' ? -1 : 1).ToList() };
    public override string SolveA(string input)
    {
        var world = Parse(input);

        foreach (var _ in Enumerable.Range(0, 2022))
        {
            world.DropShape();
        }

        return world.Height.ToString();
    }

    public override string SolveB(string input)
    {
        var world = Parse(input);

        (World.State Prev, World.State Next, List<World.State> states)? repeat = null;
        var shapes = 1000000000000L;
        long i;
        for (i = 0L; i < shapes; i++)
        {
            repeat = world.DropShape();
            if (repeat != null)
            {
                break;
            }
        }

        var (prev, next, states) = repeat!.Value;

        var distance = next.Round - prev.Round;
        var heightDiff = next.Height - prev.Height;

        var totalCycles = (shapes - prev.Round) / distance;
        var totalHeight = totalCycles * heightDiff + prev.Height;
        var remaining = (shapes - prev.Round) % distance;

        var bonusHeightAtRound = states[states.IndexOf(prev) + (int)remaining].Height - prev.Height;

        Console.WriteLine(new { totalCycles, totalHeight, remaining }.ToJson());

        return (totalHeight + bonusHeightAtRound).ToString();
    }

    public Day17()
    {
        Tests = new()
        {
            new("A",
            """
            >>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>
            """,
            "3068",
            SolveA),
            new("B",
            """
            >>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>
            """,
            "1514285714288",
            SolveB)
        };
    }
}