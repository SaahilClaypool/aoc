namespace Aoc2022;

using System.Text;
using static Aoc.Runner.Helpers;
using static Aoc2022.Facing;

enum Facing { R = 0, D = 1, L = 2, U = 3 };

public partial class Day22 : Day
{
    record Pos(int R, int C);
    class Grid
    {
        public Dictionary<int, (int RowMin, int RowMax, int ColMin, int ColMax)> SideRanges { get; set; } = new();
        readonly DefaultDictionary<int, (int, int)> colRanges = new(() => (int.MaxValue, int.MinValue));
        readonly DefaultDictionary<int, (int, int)> rowRanges = new(() => (int.MaxValue, int.MinValue));
        public Grid(DefaultDictionary<Pos, char> data)
        {
            Data = data;
            foreach (var point in data.Keys)
            {
                var (r, c) = point;
                var (rl, rr) = rowRanges[c];
                rowRanges[c] = (int.Min(r, rl), int.Max(r, rr));
                var (cl, cr) = colRanges[r];
                colRanges[r] = (int.Min(c, cl), int.Max(c, cr));
            }
        }
        public char this[Pos p]
        {
            get => Data[p];
            set => Data[p] = value;
        }

        public override string ToString()
        {
            var s = string.Empty;
            foreach (var r in Enumerable.Range(1, Data.Keys.Select(_ => _.R).Max()))
            {
                foreach (var c in Enumerable.Range(1, Data.Keys.Select(_ => _.C).Max()))
                {
                    s += Data[new(r, c)];
                }
                s += "\n";
            }
            return s;
        }

        public DefaultDictionary<Pos, char> Data { get; }

        public Pos Wrap(Pos p, Facing f)
        {
            var (rowMin, rowMax) = rowRanges[p.C];
            if (rowMin == int.MaxValue) (rowMin, rowMax) = (int.MinValue, int.MaxValue);
            var (colMin, colMax) = colRanges[p.R];
            if (colMin == int.MaxValue) (colMin, colMax) = (int.MinValue, int.MaxValue);

            static int wrap(int a, int min, int max) =>
                (a < min) ?
                    max :
                (a > max) ?
                    min :
                    a;

            if (f == R || f == L)
                return new(p.R, wrap(p.C, colMin, colMax));
            else
                return new(wrap(p.R, rowMin, rowMax), p.C);

        }

        public (Pos, Facing) WrapCube(Pos p, int side, Facing facing)
        {
            var (rmin, rmax, cmin, cmax) = SideRanges[side];
            int newSide;
            Facing newFacing;
            Pos newPos;
            if (p.R > rmax) // down
                (newSide, newFacing, newPos) = side switch
                {
                    1 => (4, D, p),
                    2 => (5, U),
                    3 => (5 , R),
                    4 => (5, D), 
                    5 => (2, U),
                    6 => (2, R),
                    _ => throw new Exception()
                };
            else if (p.R < rmin) // up
                (newSide, newFacing) = side switch
                {
                    1 => (2, D),
                    2 => (1, D),
                    3 => (1 , R),
                    4 => (1, U), 
                    5 => (4, U),
                    6 => (4, L),
                    _ => throw new Exception()
                };
            else if (p.C < cmin) // left
                (newSide, newFacing) = side switch
                {
                    1 => (3, D),
                    2 => (6, U),
                    3 => (2 , L),
                    4 => (3, L), 
                    5 => (3, U),
                    6 => (5, L),
                    _ => throw new Exception()
                };
            else if (p.C > cmax) // right
                (newSide, newFacing) = side switch
                {
                    1 => (6, L),
                    2 => (3, R),
                    3 => (4 , R),
                    4 => (6, D), 
                    5 => (6, R),
                    6 => (1, L),
                    _ => throw new Exception()
                };
        }

        public int Side(Pos p) => SideRanges.WithIndex().First(r =>
        {
            var (rmin, rmax, cmin, cmax) = r.Item1.Value;
            return p.R <= rmax && p.R >= rmin && p.C <= cmax && p.C >= cmin;
        }).Item2;
    }

    record Command(Facing Turn, int Count);

    public override string SolveA(string input)
    {
        var (grid, commands) = Parse(input);
        Log(grid.Data.Keys.ToJson());
        Log(commands.ToJson());
        var facing = U; // will flip right on first command
        var pos = grid.Wrap(new(1, int.MaxValue), L);
        foreach (var command in commands)
        {
            facing =
                facing switch
                {
                    R => command.Turn == L ? U : D,
                    L => command.Turn == L ? D : U,
                    U => command.Turn == L ? L : R,
                    D => command.Turn == L ? R : L,
                    _ => throw new Exception("bad facing")
                };
            for (var i = 0; i < command.Count; i++)
            {
                grid[pos] = facing.ToString()[0];
                var (r, c) = Dir(facing);
                var nextPos = grid.Wrap(new(pos.R + r, pos.C + c), facing);
                Log((facing, nextPos));
                if (grid[nextPos] != '#')
                    pos = nextPos;
            }
        }

        var password = 1_000L * pos.R + 4 * pos.C + (int)facing;
        Log(grid);
        return password.ToString();
    }

    (int R, int C) Dir(Facing f) =>
        f switch { R => (0, 1), L => (0, -1), U => (-1, 0), D => (1, 0), _ => throw new Exception() };

    public override string SolveB(string input)
    {
        throw new NotImplementedException();
    }

    (Grid Grid, List<Command>) Parse(string input)
    {
        var parts = input.Split("\n\n");
        var gridText = parts[0];
        Dictionary<Pos, char> data = new();

        foreach (var (row, rowNum) in gridText.Lines().WithIndex())
        {
            foreach (var (c, colNum) in row.WithIndex())
            {
                if (c != ' ')
                {
                    data[new(rowNum + 1, colNum + 1)] = c;
                }
            }
        }

        var commandText = parts[1];
        var commands = new List<Command>();
        var facing = R;
        for (var i = 0; i < commandText.Length; i++)
        {
            if (i < commandText.Length - 1 && int.TryParse(commandText[i..(i + 2)], out var count))
            {
                commands.Add(new(facing, count));
                i += 1;
            }
            else if (int.TryParse(commandText[i..(i + 1)], out count))
            {
                commands.Add(new(facing, count));
            }
            else
            {
                facing = Enum.Parse<Facing>(commandText[i..(i + 1)]);
            }
        }

        return (new(data.ToDefaultDict(() => ' ')), commands);
    }

    public Day22()
    {
        Tests = new()
        {
            new("A",
            """
                    ...#
                    .#..
                    #...
                    ....
            ...#.......#
            ........#...
            ..#....#....
            ..........#.
                    ...#....
                    .....#..
                    .#......
                    ......#.

            10R5L5R10L4R5L5
            """,
            "6032",
            SolveA),
            new("B",
            """
            1
            """,
            "",
            SolveB)
        };
    }
}