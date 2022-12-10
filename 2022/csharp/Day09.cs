namespace Aoc2022;


public partial class Day09 : Day
{
    record Pos(int X, int Y)
    {
        public bool Adjacent(Pos p) => Math.Abs(p.X - X) <= 1 && Math.Abs(p.Y - Y) <= 1;
        public Pos Follow(Pos head)
        {
            if (!Adjacent(head))
            {
                var moveX =
                        head.X < X ? -1 : // tail is right of head -> move left
                        head.X > X ? 1 : 0;
                var moveY =
                    head.Y < Y ? -1 : // tail is above head -> move down
                    head.Y > Y ? 1 : 0;
                return new(X + moveX, Y + moveY);
            }
            return this;
        }
    }
    class Knot
    {
        public Pos Pos { get; set; } = new(0, 0);
        public Knot? Next;
        public Knot Last() => Next != null ? Next.Last() : this;
    }
    record State(Knot Head, HashSet<Pos> TailLocs);

    public override string SolveA(string input)
    {
        var rope = new Knot() { Next = new() };
        var state = new State(rope, new());
        var states = input
            .Split('\n')
            .Rolling(state, Command);
        
        return states.Last().TailLocs.Count.ToString();
    }

    private State Command(State state, string line)
    {
        var dir = line.Split(" ")[0];
        var count = int.Parse(line.Split(" ")[1]);
        state.TailLocs.Add(state.Head.Last().Pos);
        Log($"COMMAND-- {line}");
        for (var i = 0; i < count; i++)
        {
            state = Step(state, dir);
            state.TailLocs.Add(state.Head.Last().Pos);
        }
        return state;
    }

    private static State Step(State state, string dir)
    {
        // first, move the head
        var newHeadPos = dir switch
        {
            "R" => state.Head.Pos with { X = state.Head.Pos.X + 1 },
            "U" => state.Head.Pos with { Y = state.Head.Pos.Y + 1 },
            "L" => state.Head.Pos with { X = state.Head.Pos.X - 1 },
            "D" => state.Head.Pos with { Y = state.Head.Pos.Y - 1 },
            _ => throw new Exception($"Invalid dir {dir}")
        };
        state.Head.Pos = newHeadPos;
        Log($"Moving head to {newHeadPos}");
        var current = state.Head;
        while(current.Next != null)
        {
            var nextPos = current.Next.Pos.Follow(current.Pos);
            Log($"Following {current.Next.Pos} to {nextPos}");
            current.Next.Pos = nextPos;
            current = current.Next;
        }
        return state;
    }

    public override string SolveB(string input)
    {
        throw new NotImplementedException();
    }

    public Day09()
    {
        Tests = new()
        {
            new("A",
            """
            R 4
            U 4
            L 3
            D 1
            R 4
            D 1
            L 5
            R 2
            """, 
            "13",
            SolveA)
        };
    }
}