using System.Text.RegularExpressions;

namespace Aoc2022;


public partial class Day11 : Day
{
    static List<Monkey> Parse(string input)
    {
        var re = MonkeyRegex();
        var matches = re.Matches(input);
        var monkeys = matches.Select(m =>
            new Monkey(
                long.Parse(m.Groups["num"].Value),
                new Queue<long>(m.Groups["items"].Value.Split(",").Select(s => long.Parse(s.Trim()))),
                long.Parse(m.Groups["divisor"].Value),
                long.Parse(m.Groups["onTrue"].Value),
                long.Parse(m.Groups["onFalse"].Value),
                m.Groups["op"].Value,
                m.Groups["opIn"].Value
            )).ToList();
        return monkeys;
    }

    record Monkey(long Number, Queue<long> Items, long Divisor, long OnTrue, long OnFalse, string Op, string OpInput)
    {
        public long Inspected { get; set; } = 0;
        public IEnumerable<Action> TakeTurn(long divisor = 3, long ring = 1)
        {
            Log($"Monkey {Number}:");
            while (Items.TryDequeue(out var item))
            {
                Log($"  Inspects {item}:");
                Inspected++;
                var worry = item;
                var input = OpInput switch
                {
                    "old" => item,
                    var c => long.Parse(c)
                };
                worry = Op switch
                {
                    "*" => worry * input,
                    "+" => worry + input,
                    _ => throw new NotImplementedException($"Invalid op {Op}")
                };
                // because we only care about divisibility - we should be able to
                // to limit size to product of all divisors (least common multiple probably)
                worry /= divisor;
                worry %= ring;
                var to = worry % Divisor == 0 ? OnTrue : OnFalse;
                Log($"Throw {worry} to {to}");
                yield return new(worry, to);
            }
        }
    }
    record Action(long WorryLevel, long ToMonkey);
    record State(Dictionary<long, Monkey> Monkeys)
    {
        public void Round(long divisor, long ring)
        {
            foreach (var monkey in Monkeys.Values.OrderBy(m => m.Number))
                foreach (var action in monkey.TakeTurn(divisor, ring))
                    Monkeys[action.ToMonkey].Items.Enqueue(action.WorryLevel);
        }
    }

    static string Simulate(State state, int rounds, long divisor = 3, long ring = int.MaxValue)
    {
        foreach (var round in Enumerable.Range(0, rounds))
        {
            state.Round(divisor, ring);
            Log($"--- Round {round} ---");
            foreach (var m in state.Monkeys.Values)
            {
                Log($"Monkey {m.Number}: {string.Join(", ", m.Items)}");
            }
        }
        return state.Monkeys.Values
            .Select(m => m.Inspected)
            .OrderDescending()
            .Take(2)
            .Aggregate((a, b) => a * b)
            .ToString();

    }

    public override string SolveA(string input)
    {
        var state = new State(Parse(input).ToDictionary(m => m.Number));
        return Simulate(state, 20);
    }

    public override string SolveB(string input)
    {
        var isTest = IsTest;
        // too big for logging
        var state = new State(Parse(input).ToDictionary(m => m.Number));
        var ring = state.Monkeys.Values.Select(v => v.Divisor).Aggregate((a, b) => a * b);
        Log($"Ring: {ring}");
        SetTest(false);
        var result = Simulate(state, 10000, divisor: 1, ring: ring);
        SetTest(IsTest);
        Log(string.Join("\n", state.Monkeys.Values.Select(m => $"Monkey {m.Number} {m.Inspected}")));
        return result;
    }

    [GeneratedRegex("""
    Monkey (?<num>\d+):
      Starting items: (?<items>.*)
      Operation: new = old (?<op>.) (?<opIn>.*)
      Test: divisible by (?<divisor>\d+)
        If true: throw to monkey (?<onTrue>\d+)
        If false: throw to monkey (?<onFalse>\d+)
    """, RegexOptions.Multiline)]
    private static partial Regex MonkeyRegex();

    public Day11()
    {
        Tests = new()
        {
            new("A",
                """
                Monkey 0:
                  Starting items: 79, 98
                  Operation: new = old * 19
                  Test: divisible by 23
                    If true: throw to monkey 2
                    If false: throw to monkey 3
                
                Monkey 1:
                  Starting items: 54, 65, 75, 74
                  Operation: new = old + 6
                  Test: divisible by 19
                    If true: throw to monkey 2
                    If false: throw to monkey 0
                
                Monkey 2:
                  Starting items: 79, 60, 97
                  Operation: new = old * old
                  Test: divisible by 13
                    If true: throw to monkey 1
                    If false: throw to monkey 3
                
                Monkey 3:
                  Starting items: 74
                  Operation: new = old + 3
                  Test: divisible by 17
                    If true: throw to monkey 0
                    If false: throw to monkey 1
                """,
                "10605",
                SolveA
            ),
            new(
                "B",
                """
                Monkey 0:
                  Starting items: 79, 98
                  Operation: new = old * 19
                  Test: divisible by 23
                    If true: throw to monkey 2
                    If false: throw to monkey 3
                
                Monkey 1:
                  Starting items: 54, 65, 75, 74
                  Operation: new = old + 6
                  Test: divisible by 19
                    If true: throw to monkey 2
                    If false: throw to monkey 0
                
                Monkey 2:
                  Starting items: 79, 60, 97
                  Operation: new = old * old
                  Test: divisible by 13
                    If true: throw to monkey 1
                    If false: throw to monkey 3
                
                Monkey 3:
                  Starting items: 74
                  Operation: new = old + 3
                  Test: divisible by 17
                    If true: throw to monkey 0
                    If false: throw to monkey 1
                """,
                "2713310158",
                SolveB
            )
        };
    }
}