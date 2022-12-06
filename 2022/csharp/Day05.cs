using System.Text.RegularExpressions;

namespace Aoc2022;


public partial class Day05 : Day
{
    private readonly char EMPTY = '\0';

    public override string SolveA(string input)
    {
        var state = Parse(input);
        foreach (var command in state.Commands)
        {
            foreach (var i in Enumerable.Range(0, command.Count))
            {
                var c = state.Stacks[command.From].Pop();
                state.Stacks[command.To].Push(c);
            }
        }

        var result = "";
        foreach (var stack in state.Stacks)
        {
            result += stack.Peek();
        }
        return result;
    }

    public override string SolveB(string input)
    {
        var state = Parse(input);
        foreach (var command in state.Commands)
        {
            var queue = new List<char>();
            foreach (var i in Enumerable.Range(0, command.Count))
            {
                var c = state.Stacks[command.From].Pop();
                queue.Add(c);
            }
            foreach (var c in queue.Reverse<char>())
            {
                state.Stacks[command.To].Push(c);
            }
        }

        var result = "";
        foreach (var stack in state.Stacks)
        {
            result += stack.Peek();
        }
        return result;
    }

    private State Parse(string input)
    {
        var sections = input.Split("\n\n");
        var (stackInput, commandInput) = (sections[0], sections[1]);

        var rows = stackInput.Split('\n').Select(line =>
        {
            var columns = line.Chunk(4);
            return columns.Select(col => col[0] == '[' ? col[1] : EMPTY);
        });
        List<List<char>> invertedStacks = new();
        foreach (var row in rows)
        {
            var i = 0;
            foreach (var col in row)
            {
                if (i >= invertedStacks.Count)
                {
                    invertedStacks.Add(new());
                }
                invertedStacks[i].Add(col);
                i++;
            }
        }
        var stacks = invertedStacks.Select(stack => new Stack<char>(stack.Where(i => i != EMPTY).Reverse<char>())).ToList();

        var re = CommandRegex();
        var commands = commandInput
            .Split('\n')
            .Where(line => re.IsMatch(line))
            .Select(line =>
            {
                Log(line);
                var match = re.Match(line);
                return new Command(
                    int.Parse(match.Groups[1].Value),
                    int.Parse(match.Groups[2].Value) - 1,
                    int.Parse(match.Groups[3].Value) - 1);
            }).ToList();

        var state = new State(stacks, commands);
        Log(state);
        return state;
    }

    record State(List<Stack<char>> Stacks, List<Command> Commands)
    {
        public override string ToString()
        {
            var stacks = string.Join("\n",
                 Stacks.Select((stack, i) =>
                    $"{i}: " + string.Join(" ", stack.Select(c => $"[{c}]"))));
            return $"""
            {stacks}

            {string.Join("\n", Commands)}
            """;
        }
    };
    record Command(int Count, int From, int To);

    public Day05()
    {
        Tests = new()
        {
            new("A",
            """
                [D]    
            [N] [C]    
            [Z] [M] [P]
            1   2   3 

            move 1 from 2 to 1
            move 3 from 1 to 3
            move 2 from 2 to 1
            move 1 from 1 to 2
            """,
            "CMZ",
            SolveA),
            new("B",
            """
                [D]    
            [N] [C]    
            [Z] [M] [P]
            1   2   3 

            move 1 from 2 to 1
            move 3 from 1 to 3
            move 2 from 2 to 1
            move 1 from 1 to 2
            """,
            "MCD",
            SolveB)
        };
    }

    [GeneratedRegex("move (.*) from (.*) to (.*)")]
    private static partial Regex CommandRegex();
}