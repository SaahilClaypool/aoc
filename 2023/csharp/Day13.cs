namespace Aoc.Solutions.Y2023;

public class Day13 : Day
{
    string Flip(string s) =>
        s.Lines()
        .SelectMany((line, row) =>
            line.Select((c, col) => (row, col, c))
        ).GroupBy(x => x.col)
        .Select(x => string.Join("", x.OrderBy(x => x.row).Select(x => x.c)))
        .Pipe(x => string.Join("\n", x));

    bool IsMirrorLine(string line, int l)
    {
        var left = line[..l].Reverse().Pipe(x => string.Join("", x));
        var right = line[l..];
        if (right.Length > left.Length)
        {
            var temp = left;
            left = right;
            right = temp;
        }
        return left.StartsWith(right);
    }

    (int? MirrorRow, int? MirrorCol) FindMirror(string input, int? ignoreRow = null, int? ignoreCol = null) =>
        (FindMirrorRow(input, ignoreRow), FindMirrorColumn(input, ignoreCol));

    int? FindMirrorColumn(string input, int? ignore = null)
    {
        var lines = input.Lines();
        for (var i = 1; i < lines[0].Length; i++)
        {
            if (i == ignore) continue;
            if (lines.All(l => IsMirrorLine(l, i)))
            {
                return i;
            }
        }
        return null;
    }

    int? FindMirrorRow(string input, int? ignore = null)
    {
        var flipped = Flip(input);
        return FindMirrorColumn(flipped, ignore);
    }

    (string, int) FindSmudge(string input)
    {
        var (orow, ocol) = FindMirror(input);
        Console.WriteLine($"og: {(orow, ocol)}");
        var smudge = PossibleSmudes(input).Select(x =>
        {
            var (mrow, mcol) = FindMirror(x, orow, ocol);
            if (mrow == orow)
                mrow = null;
            if (mcol == ocol)
                mcol = null;
            var validSmudge = mrow != null || mcol != null;
            if (validSmudge)
            {
                Console.WriteLine($"Found {(mrow, mcol)}");
            }
            return new { Smudge = x, Valid = validSmudge, Row = mrow, Col = mcol };
        })
        .FirstOrDefault(x => x.Valid);
        if (smudge is null)
            throw new Exception($"Failed to find smudge for\n{input}");
        return (smudge.Smudge, smudge.Row != null ? smudge.Row.Value * 100 : smudge.Col!.Value);
    }

    IEnumerable<string> PossibleSmudes(string input)
    {
        foreach (var (c, i) in input.WithIndex())
        {
            if (c != '.' && c != '#') continue;
            var newC = (c == '.') ? '#' : '.';
            yield return $"{input[..i]}{newC}{input[(i + 1)..]}";
        }
    }

    public int? Score(string mirror)
    {
        if (FindMirrorColumn(mirror) is var c && c != null)
        {
            return c.Value;
        }
        else if (FindMirrorRow(mirror) is var r && r != null)
        {
            return r.Value * 100;
        }
        else
        {
            return null;
        }
    }

    public override string SolveA(string input)
    {
        var total = 0;
        foreach (var diagram in input.Split("\n\n"))
        {
            var score = Score(diagram);
            if (score is null)
            {
                Console.WriteLine($"Warning - no score for:\n{diagram}");
                continue;
            }
            total += score.Value;
        }
        return total.ToString();
    }

    public override string SolveB(string input)
    {
        var total = 0;
        foreach (var (diagram, i) in input.Split("\n\n").WithIndex())
        {
            Console.WriteLine($"IDX: {i}");
            var (smudge, score) = FindSmudge(diagram);
            Log($"Smudge for\n{diagram}\n--\n{smudge}");
            total += score;
        }
        return total.ToString();
    }

    public Day13()
    {
        Tests = new()
        {

            new("a", SampleA, "405", SolveA),
            new("a2", SampleA2, "1200", SolveA),
            new("b", SampleA, "400", SolveB),
            new("b", SampleB, idea, x => FindSmudge(x).Item1),
            new("Flip",
                """
                ab
                cd
                """,
                """
                ac
                bd
                """,
                Flip
            ),
            new("IsMirrorLine", "abccba", "True", x => IsMirrorLine(x, 3).ToString()),
            new("IsMirrorLine", "abccbaxxx", "True", x => IsMirrorLine(x, 3).ToString()),
            new("IsMirrorLine", "xabccba", "False", x => IsMirrorLine(x, 3).ToString()),
            new("smudges", "#.#", "3", x => PossibleSmudes(x).Count().ToString())
        };
    }

    static string SampleA = """
    #.##..##.
    ..#.##.#.
    ##......#
    ##......#
    ..#.##.#.
    ..##..##.
    #.#.##.#.

    #...##..#
    #....#..#
    ..##..###
    #####.##.
    #####.##.
    ..##..###
    #....#..#
    """;

    static string SampleA2 = """
    ##.#####.#####...
    ##.#.#####...####
    ..#..##.#.#.##.#.
    ###.#....##.#....
    ..##.#...........
    ####.#.##..#....#
    .####..#..#...##.
    ###..####.#.##.#.
    #######..#...#...
    ######..#....##..
    ..###.#.##.......
    ...#.####...###.#
    ...#.####...###.#
    """;

    static string SampleB = """
    #..####.##.##
    .##.##.####.#
    #######.##.##
    ####.........
    #..##..####..
    ....##.#..#.#
    .##..#.####.#
    ....#.#....#.
    .....##....##
    .##.##.#..#.#
    #..##..#..#..
    ####.######.#
    .....#.####.#
    .##.##......#
    #..##........
    """;

    static string idea = """
    #..####.##.##
    .##.##.####.#
    #######.##.##
    ####.........
    #..##..####..
    ....##.#..#.#
    .##..#.####.#
    ....#.#....#.
    .....##....##
    .##.##.#..#.#
    #..##..#..#..
    ####.#.####.#
    .....#.####.#
    .##.##......#
    #..##........
    """;
}