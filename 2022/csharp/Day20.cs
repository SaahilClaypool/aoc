namespace Aoc2022;


public partial class Day20 : Day
{
    class Node
    {
        public long Value { get; set; }
        public Node OGPrev { get; set; } = null!;
        public Node OGNext { get; set; } = null!;
        public Node Next { get; set; } = null!;
        public Node Prev { get; set; } = null!;
        public IEnumerable<Node> Iterate()
        {
            yield return this;
            var current = Next;
            while (current != this)
            {
                yield return current;
                current = current.Next;
            }
        }
        public override string ToString() => Value.ToString();

        public void MoveRight()
        {
            var next = Next;
            var prev = Prev;
            var nextNext = next.Next;
            prev.Next = next;
            next.Prev = prev;
            next.Next = this;
            Prev = next;
            Next = nextNext;
            nextNext.Prev = this;

        }

        public void MoveLeft()
        {
            var next = Next;
            var prev = Prev;
            var prevPrev = prev.Prev;
            // a <> b <> c -> a <> c <> b
            prev.Next = next;
            next.Prev = prev;
            prevPrev.Next = this;
            Prev = prevPrev;
            Next = prev;
            prev.Prev = this;
        }
    };

    public override string SolveA(string input)
    {
        var root = Parse(input);
        Log($"OG:  {string.Join(", ", root.Iterate().Select(v => v.Value))}");
        Node current = Mix(root);
        return Decode(root);
    }

    private static string Decode(Node root)
    {
        List<long> nums = new();
        var zero = root.Iterate().First(r => r.Value == 0);
        Node current = zero;
        for (var i = 1; i <= 3_000; i++)
        {
            current = current.Next;
            if (i % 1_000 == 0)
            {
                nums.Add(current.Value);
            }
        }
        Log($"Nums: {string.Join(", ", nums)}");
        return nums.Sum().ToString();
    }

    private static Node Mix(Node root)
    {
        var total = root.Iterate().Count();
        var current = root;
        var first = true;
        while (first || current != root)
        {
            first = false;
            var val = current.Value;
            if (val > 0)
            {
                // var rounds = (val % total);
                /**
                0 a b c d
                1 a b d c
                2 a d b c
                3 d a b c
                4 c a b d == a b d c

                */
                var rounds = val % (total - 1);
                for (var i = 0; i < rounds; i++)
                {
                    current.MoveRight();
                }
            }
            else
            {
                var rounds = -val % (total - 1);
                for (var i = 0; i < rounds; i++)
                {
                    current.MoveLeft();
                }
            }
            current = current.OGNext;
        }

        return current;
    }

    public override string SolveB(string input)
    {
        var root = Parse(input);
        foreach (var node in root.Iterate())
        {
            node.Value *= 811589153;
        }
        Log($"OG:  {string.Join(", ", root.Iterate().Select(v => v.Value))}");
        foreach (var i in Enumerable.Range(0, 10))
        {
            Mix(root);
            Log($"M{i}: {string.Join(", ", root.Iterate().Select(v => v.Value))}");
        }
        return Decode(root);
    }

    Node Parse(string input)
    {
        var nums = input.Lines().Select(long.Parse).ToList();
        Node root = null!;
        Node current = null!;
        for (var i = 0; i < nums.Count; i++)
        {
            var t = new Node
            {
                Value = nums[i],
                OGPrev = current,
                Prev = current,
            };
            if (current != null)
            {
                current.Next = t;
                current.OGNext = t;
            }
            root ??= t;
            current = t;
        }
        current.Next = root;
        current.OGNext = root;
        root.Prev = current;
        root.OGPrev = current;
        return root;
    }

    public Day20()
    {
        Tests = new()
        {
            new("A",
            """
            1
            2
            -3
            3
            -2
            0
            4
            """,
            "3",
            SolveA),
            new("B",
            """
            1
            2
            -3
            3
            -2
            0
            4
            """,
            "1623178306",
            SolveB)
        };
    }
}