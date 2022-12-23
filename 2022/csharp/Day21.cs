namespace Aoc2022;


public partial class Day21 : Day
{
    enum Operation { Add, Subtract, Multiply, Divide }
    class Monkey
    {
        public string Name { get; set; } = string.Empty;
        public long? Value { get; set; }
        public string Left { get; set; } = string.Empty;
        public string Right { get; set; } = string.Empty;
        public Operation? Op { get; set;}

        public long Resolve(Dictionary<string, Monkey> monkeys, out HashSet<string> dependencies)
        {
            if (Value != null)
            {
                dependencies = new HashSet<string>();
                return Value.Value;
            }
            else
            {
                var a = monkeys[Left].Resolve(monkeys, out var leftDepends);
                var b = monkeys[Right].Resolve(monkeys, out var rightDepends);
                dependencies = leftDepends.Concat(rightDepends).ToHashSet();
                var val = Op switch
                {
                    Operation.Add => a + b,
                    Operation.Divide => a / b,
                    Operation.Multiply => a * b,
                    Operation.Subtract => a - b,
                    _ => throw new InvalidOperationException("No such enum")
                };
                Value = val;
                return val;
            }
        }
    }

    Dictionary<string, Monkey> ParseMonkeys(string input)
    {
        return input.Lines().Select(line =>
        {
            var parts = line.Split(":");
            var name = parts[0];
            var op = parts[1].Trim();
            long? value = long.TryParse(op, out var v) ? v : null;
            return new Monkey()
            {
                Name = name,
                Value = value,
                Left = value == null ? op.Split(" ")[0].Trim() : string.Empty,
                Right = value == null ? op.Split(" ")[2].Trim() : string.Empty,
                Op = value == null ? op.Split(" ")[1].Trim() switch
                {
                    "/" => Operation.Divide,
                    "+" => Operation.Add,
                    "-" => Operation.Subtract,
                    "*" => Operation.Multiply,
                    var s => throw new NotImplementedException(s)
                } : null,
            };
        }).ToDictionary(k => k.Name);
    }

    public override string SolveA(string input)
    {
        var m = ParseMonkeys(input);
        return m["root"].Resolve(m).ToString();
    }

    public override string SolveB(string input)
    {
        // a + b = root
        // c + d = a
        // h + e = c
        var m = ParseMonkeys(input);
        var root = m["root"];
        m.Remove("humn");

        static Operation Invert(Operation o) => o switch
        {
            Operation.Add => Operation.Subtract,
            Operation.Subtract => Operation.Add,
            Operation.Multiply => Operation.Divide,
            Operation.Divide => Operation.Multiply,
            _ => throw new Exception()
        };

        var equations = m.Values.SelectMany(m =>
        {
            if (m.Value != null)
            {
                return new Monkey[] { m };
            }
            return new Monkey[]
            {
                new() { Name = m.Left, Op = Invert(m.Op!.Value), Left = m.Name, Right = m.Right },
                new() { Name = m.Right, Op = Invert(m.Op!.Value), Left = m.Name, Right = m.Left },
            };
        })
        .Append(new() { Name = "root", Left = root.Left, Right = root.Right, Op = Invert(root.Op!.Value) })
        .ToList();
        root.Value = 0;

        var newMonkeys = equations.GroupBy(_ => _.Name).ToDictionary(_ => _.Key, _ => _.First());

        Dictionary<string, long> solved = equations.Where(_ => _.Value.HasValue).GroupBy(_ => _.Name).ToDictionary(_ => _.Key, _ => _.First().Value!.Value);
        equations.RemoveAll(e => solved.ContainsKey(e.Name));

        while (!solved.ContainsKey("humn"))
        {
            foreach (var eq in equations)
            {
                if (solved.ContainsKey(eq.Left) && solved.ContainsKey(eq.Right))
                {
                    solved[eq.Name] = eq.Resolve(newMonkeys);
                }
            }
        }
        
        return newMonkeys["humn"].Resolve(newMonkeys).ToString();
    }

    public Day21()
    {
        Tests = new()
        {
            new("A",
            """
            root: pppw + sjmn
            dbpl: 5
            cczh: sllz + lgvd
            zczc: 2
            ptdq: humn - dvpt
            dvpt: 3
            lfqf: 4
            humn: 5
            ljgn: 2
            sjmn: drzm * dbpl
            sllz: 4
            pppw: cczh / lfqf
            lgvd: ljgn * ptdq
            drzm: hmdt - zczc
            hmdt: 32
            """,
            "152",
            SolveA),
            new("B",
            """
            root: pppw + sjmn
            dbpl: 5
            cczh: sllz + lgvd
            zczc: 2
            ptdq: humn - dvpt
            dvpt: 3
            lfqf: 4
            humn: 5
            ljgn: 2
            sjmn: drzm * dbpl
            sllz: 4
            pppw: cczh / lfqf
            lgvd: ljgn * ptdq
            drzm: hmdt - zczc
            hmdt: 32
            """,
            "301",
            SolveB)
        };
    }
}