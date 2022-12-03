namespace Aoc.Solutions.Day18;

public class Day18 : Day
{
    Num ParseNumber(string str) => ParseNumber(str, out var _);
    Num ParseNumber(string str, out int consumed)
    {
        Log(str);
        if (str.StartsWith('['))
        {
            var left = ParseNumber(str[1..], out var leftConsumed);
            var right = ParseNumber(str[(leftConsumed + 2)..], out var rightConsumed);
            consumed = leftConsumed + rightConsumed + 3; // left bracket + comma + right bracket

            var p = new Num.Pair(left, right);
            p.Left.Parent = p;
            p.Right.Parent = p;
            return p;
        }
        else
        {
            var part = str.Split(']')[0].Split(',')[0];
            consumed = part.Length; // use the elength of the literal
            return new Num.Literal(int.Parse(part), null);
        }
    }

    Num[] Parse(string s) =>
        s
        .Split('\n')
        .Select(ParseNumber)
        .ToArray();

    public static Num Add(Num a, Num b) => new Num.Pair(a, b);
    public static Num Reduce(Num num)
    {
        bool hasChanged = true;
        while (hasChanged)
        {
            hasChanged = false;
            num = Explode(num, out var changed);
            if (changed)
            {
                hasChanged = true;
                continue;
            }
            num = Split(num, out changed);
            if (changed)
            {
                hasChanged = true;
                continue;
            }
        }
        return num;
    }

    private static Num Split(Num num, out bool changed)
    {
        changed = false;
        foreach (var (n, depth) in Traverse(num, 0))
        {
            if (n is Num.Literal lit && lit.Val >= 10)
            {
                changed = true;
                var newPair = new Num.Pair(
                    new Num.Literal(lit.Val / 2, null),
                    new Num.Literal(lit.Val % 2 == 0 ? lit.Val / 2 : (lit.Val + 1) / 2, null))
                {
                    Parent = lit.Parent
                };
                if (lit.IsLeft())
                {
                    lit.Parent!.Left = newPair;
                }
                else if (lit.IsRight())
                {
                    lit.Parent!.Right = newPair;
                }
                else
                {
                    return newPair;
                }
                return num;
            }
        }
        return num;
    }

    private static Num Explode(Num num, out bool changed)
    {
        changed = false;
        foreach (var (n, depth) in Traverse(num, 0))
        {
            if (n is Num.Pair p && depth == 4)
            {
                changed = true;
                // the pair's left value is added to the first regular number to the left of the exploding pair (if any)
                if (p.GetLeft() is Num.Literal l)
                {
                    l.Val += ((Num.Literal)p.Left).Val;
                }
                if (p.GetRight() is Num.Literal r)
                {
                    r.Val += ((Num.Literal)p.Right).Val;
                }
                if (p.Parent!.Left == p)
                {
                    p.Parent!.Left = new Num.Literal(0, p.Parent);
                }
                else
                {
                    p.Parent!.Right = new Num.Literal(0, p.Parent);
                }
                return num;
            }
        }
        return num;
    }

    static List<(Num Num, int Depth)> Traverse(Num num, int depth) =>
        num.Match(
            literal => new() { (literal, depth) },
            pair =>
                Traverse(pair.Left, depth + 1)
                    .Concat(new List<(Num, int)> { (pair, depth) })
                    .Concat(Traverse(pair.Right, depth + 1))
                    .ToList()
        );

    public static Num Sum(IList<Num> nums) => nums.Aggregate((a, b) => Add(a, b).Then(Reduce));
    public override string SolveA(string input)
    {
        var inp = Parse(input);
        return Sum(inp).Magnitude().ToString();
    }

    public override string SolveB(string input)
    {
        var inp = Parse(input);
        var pairs = inp.AllPairs().Select(pair => (Left: Clone(pair.Left), Right: Clone(pair.Right)));
        var bestPair = pairs
            .MaxBy(p => Add(p.Left, p.Right).Then(Reduce).Magnitude());
        Log(bestPair);
        return Add(bestPair.Left, bestPair.Right).Then(Reduce).Magnitude().ToString();
    }

    private Num Clone(Num num) => ParseNumber(num.ToString());

    public Day18()
    {
        Tests = new()
        {
            new("Explode", @"[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]", i => ParseNumber(i).Then(Reduce).ToString()),
            new("Split", @"11", "[5,6]", i => ParseNumber(i).Then(Reduce).ToString()),
            new("Sum", @"[1,1]
[2,2]
[3,3]
[4,4]
[5,5]
[6,6]", "[[[[5,0],[7,4]],[5,5]],[6,6]]", i => Parse(i).Then(Sum).Then(n => n.ToString()!)),
            new("Magnitude", @"[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]", "3488", i => Parse(i).Then(Sum).Magnitude().ToString()),
            new("Sum Sample", @"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]
[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]
[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]
[7,[5,[[3,8],[1,4]]]]
[[2,[2,2]],[8,[8,1]]]
[2,9]
[1,[[[9,3],9],[[9,0],[0,7]]]]
[[[5,[7,4]],7],1]
[[[[4,2],2],6],[8,7]]", "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]", i => Parse(i).Then(Sum).ToString()),
            new("Sum2", @"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]", "[[[[6,6],[7,6]],[[7,7],[7,0]]],[[[7,7],[7,7]],[[7,8],[9,9]]]]", i => Parse(i).Then(Sum).ToString()),
            new("A", @"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]", "4140", SolveA),
            new("B", @"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]", "3993", SolveB)
        };
    }

}

[ExhaustiveMatch.GenerateMatch]
public abstract class Num
{
    public bool IsLeft() => Parent?.Left == this;
    public bool IsRight() => Parent?.Right == this;
    public Pair? Parent { get; set; }

    public class Literal : Num
    {
        public Literal(int val, Pair? parent)
        {
            Val = val;
            Parent = parent;
        }

        public int Val { get; set; }

        public override Literal LeftMost() => this;

        public override Literal RightMost() => this;

    }

    public class Pair : Num
    {
        public Num Left { get; set; }
        public Num Right { get; set; }

        public Pair(Num left, Num right)
        {
            Left = left;
            Right = right;
            left.Parent = this;
            right.Parent = this;
        }

        public override Literal RightMost()
        {
            var cur = Right;
            while (cur is Pair p)
            {
                cur = p.Right;
            }
            return (Literal)cur;
        }

        public override Literal LeftMost()
        {
            var cur = Left;
            while (cur is Pair p)
            {
                cur = p.Left;
            }
            return (Literal)cur;
        }
    }

    public abstract Literal RightMost();
    public abstract Literal LeftMost();

    public Literal? GetLeft()
    {
        if (Parent == null)
        {
            return null;
        }
        else if (Parent.Right == this)
        {
            return Parent.Left.RightMost();
        }
        else
        {
            return Parent.GetLeft();
        }
    }

    public Literal? GetRight()
    {
        if (Parent == null)
        {
            return null;
        }
        else if (Parent.Left == this)
        {
            return Parent.Right.LeftMost();
        }
        else
        {
            return Parent.GetRight();
        }
    }

    public override string ToString() =>
        this.Match(l => l.Val.ToString(), p => $"[{p.Left},{p.Right}]");

    public int Magnitude() =>
        this.Match(l => l.Val, p => p.Left.Magnitude() * 3 + p.Right.Magnitude() * 2);
}