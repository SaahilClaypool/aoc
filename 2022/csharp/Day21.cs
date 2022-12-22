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

        public long Resolve(Dictionary<string, Monkey> monkeys)
        {
            if (Value != null)
            {
                return Value.Value;
            }
            else
            {
                var a = monkeys[Left].Resolve(monkeys);
                var b = monkeys[Right].Resolve(monkeys);
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
        var m = ParseMonkeys(input);
        var root = m["root"];
        root.Value = 0;
        root.Op = Operation.Divide;
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