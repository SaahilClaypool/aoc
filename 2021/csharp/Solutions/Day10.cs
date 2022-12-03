namespace Aoc.Solutions.Day10;

public class Chunk
{
    static readonly Dictionary<char, char> Delims = new()
    {
        ['['] = ']',
        ['('] = ')',
        ['{'] = '}',
        ['<'] = '>',
    };

    public static (bool isValid, char invalidClose, Stack<char> stack) Parse(string str)
    {
        Stack<char> stack = new();
        var consumed = 0;
        var valid = new List<char>();
        foreach (var c in str)
        {
            // open
            if (Delims.ContainsKey(c))
            {
                stack.Push(Delims[c]);
            }
            else
            {
                if (stack.TryPop(out var expected) && expected == c)
                {
                    valid.Add(c);
                }
                else
                {
                    return (false, c, stack);
                }
            }
            consumed += 1;
        }
        return (true, '\0', stack);
    }

    internal static long Score(char invalidClose) =>
        invalidClose switch
        {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            _ => throw new Exception("Invalid")
        };

    internal static long ScoreCompletion(Stack<char> stack) =>
        stack.Aggregate(0L, (state, c) =>
        {
            long score = c switch
            {
                ')' => 1,
                ']' => 2,
                '}' => 3,
                '>' => 4,
                _ => throw new Exception("Invalid")
            };
            return state * 5L + score;
        });
}

public class Day10 : Day
{
    public override string SolveA(string input)
    {
        var lines = input.Split('\n');
        var parsed = lines.Select(Chunk.Parse);
        var scored =
            parsed
            .Where(c => !c.isValid)
            .Select(c => Chunk.Score(c.invalidClose))
            .Sum();
        return scored.ToString();
    }

    public override string SolveB(string input)
    {
        var lines = input.Split('\n');
        var parsed = lines.Select(Chunk.Parse);
        var valid =
            parsed
            .Where(c => c.isValid);
        var scored =
            valid
            .Select(c => Chunk.ScoreCompletion(c.stack))
            .OrderByDescending(score => score)
            .ToList();
        return scored[scored.Count / 2].ToString();
    }

    public Day10()
    {
        Tests = new()
        {
            new("A", @"[({(<(())[]>[[{[]{<()<>>
[(()[<>])]({[<{<<[]>>(
{([(<{}[<>[]}>{[]{[(<()>
(((({<>}<{<{<>}{[]{[]{}
[[<[([]))<([[{}[[()]]]
[{[{({}]{}}([{[{{{}}([]
{<[[]]>}<{[{[{[]{()[[[]
[<(<(<(<{}))><([]([]()
<{([([[(<>()){}]>(<<{{
<{([{{}}[<[[[<>{}]]]>[]]", "26397", SolveA),
            new("A", @"[({(<(())[]>[[{[]{<()<>>
[(()[<>])]({[<{<<[]>>(
{([(<{}[<>[]}>{[]{[(<()>
(((({<>}<{<{<>}{[]{[]{}
[[<[([]))<([[{}[[()]]]
[{[{({}]{}}([{[{{{}}([]
{<[[]]>}<{[{[{[]{()[[[]
[<(<(<(<{}))><([]([]()
<{([([[(<>()){}]>(<<{{
<{([{{}}[<[[[<>{}]]]>[]]", "288957", SolveB)
        };
    }
}