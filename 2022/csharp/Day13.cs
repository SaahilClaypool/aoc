namespace Aoc2022;


public partial class Day13 : Day
{
    record Data()
    {
        public bool? InOrder(Data right)
        {
            Log($"\t- Compare {this} vs {right}");
            if (this is IntData leftInt && right is IntData rightInt)
            {
                if (leftInt.Value < rightInt.Value)
                {
                    return true;
                }
                else if (leftInt.Value > rightInt.Value)
                {
                    return false;
                }
                return null;
            }

            if (this is ListData leftData && right is ListData rightData)
            {
                foreach (var (leftI, rightI) in leftData.Value.Zip(rightData.Value))
                {
                    var inOrder = leftI.InOrder(rightI);
                    if (inOrder != null)
                    {
                        return inOrder;
                    }
                }
                return
                    leftData.Value.Count < rightData.Value.Count ? true :
                    leftData.Value.Count > rightData.Value.Count ? false :
                    null;
            }
            else
            {
                if (this is IntData intData && right is ListData listData)
                {
                    return new ListData(new List<Data> { intData }).InOrder(listData);
                }
                else if (right is IntData intData2 && this is ListData listData2)
                {
                    return listData2.InOrder(new ListData(new List<Data> { intData2 }));
                }
            }
            throw new NotImplementedException();
        }

        public record IntData(int Value) : Data()
        {
            public override string ToString() => Value.ToString();
        }
        public record ListData(List<Data> Value) : Data()
        {
            public override string ToString() => $"[{string.Join(",", Value)}]";
        }
    }
    public override string SolveA(string input)
    {
        var data = Parse(input);
        var inOrder = data
            .WithIndex()
            .Where(pair =>
            {
                Log($"Comparing Pair #{pair.Item2}");
                var inOrder = pair.Item1.Left.InOrder(pair.Item1.Right);
                return inOrder == true;
            });
        return inOrder
            .Sum(p => p.Item2 + 1).ToString();
    }

    public override string SolveB(string input)
    {
        throw new NotImplementedException();
    }

    List<(Data Left, Data Right)> Parse(string input) =>
        input.Split("\n\n")
            .Select(pair =>
            {
                var left = pair.Lines().First();
                var right = pair.Lines().Skip(1).First();
                return (ParseStr(left).Data, ParseStr(right).Data);
            })
            .ToList();

    private (Data Data, int Consumed) ParseStr(string content)
    {
        if (content.StartsWith("["))
        {
            var items = new List<Data>();
            // list
            var index = 1;
            while (index < content.Length)
            {
                if (content[index] == ',')
                {
                    index++;
                    continue;
                }
                if (content[index] == ']')
                {
                    index++;
                    break;
                }
                var (data, consumed) = ParseStr(content[index..]);
                items.Add(data);
                index += consumed;
            }
            return (new Data.ListData(items), index);
        }
        else
        {
            var comma = content.IndexOf(",");
            var bracket = content.IndexOf("]");
            var end = int.Min(
                int.Min(content.Length, comma >= 0 ? comma : int.MaxValue),
                bracket >= 0 ? bracket : int.MaxValue
            );
            // consume length + comma
            return (new Data.IntData(int.Parse(content[..end])), end);
        }
    }

    public Day13()
    {
        Tests = new()
        {
            new("Parse",
            "[[1],4]",
            "[[1],4]",
            input => ParseStr(input).Data.ToString()),
            new("ParseA",
            """
            [1,1,3,1,1]
            [1,1,5,1,1]

            [[1],[2,3,4]]
            [[1],4]

            [9]
            [[8,7,6]]

            [[4,4],4,4]
            [[4,4],4,4,4]

            [7,7,7,7]
            [7,7,7]

            []
            [3]

            [[[]]]
            [[]]

            [1,[2,[3,[4,[5,6,7]]]],8,9]
            [1,[2,[3,[4,[5,6,0]]]],8,9]
            """,
            """
            [1,1,3,1,1]
            [1,1,5,1,1]

            [[1],[2,3,4]]
            [[1],4]

            [9]
            [[8,7,6]]

            [[4,4],4,4]
            [[4,4],4,4,4]

            [7,7,7,7]
            [7,7,7]

            []
            [3]

            [[[]]]
            [[]]

            [1,[2,[3,[4,[5,6,7]]]],8,9]
            [1,[2,[3,[4,[5,6,0]]]],8,9]
            """,
            input =>
            {
                var parsed = Parse(input);
                return string.Join("\n\n", parsed.Select(
                    pair => string.Join("\n", new[] { pair.Left.ToString(), pair.Right.ToString() })
                ));
            }),
            new("ParseInput",
                "", "True",
                _ =>
                {
                    var input = GetInput();
                    var parsed = Parse(input);
                    Log(input);
                    var output = string.Join("\n\n", parsed.Select(
                        pair => string.Join("\n", new[] { pair.Left.ToString(), pair.Right.ToString() })
                    ));
                    Log(output);
                    return (output == input).ToString();
                }),
            new("A",
            """
            [1,1,3,1,1]
            [1,1,5,1,1]

            [[1],[2,3,4]]
            [[1],4]

            [9]
            [[8,7,6]]

            [[4,4],4,4]
            [[4,4],4,4,4]

            [7,7,7,7]
            [7,7,7]

            []
            [3]

            [[[]]]
            [[]]

            [1,[2,[3,[4,[5,6,7]]]],8,9]
            [1,[2,[3,[4,[5,6,0]]]],8,9]
            """,
            "13",
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