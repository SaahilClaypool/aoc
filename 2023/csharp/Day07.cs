using System.Numerics;

namespace Aoc.Solutions.Y2023;

public class Day07 : Day
{
    static bool JLow = false;
    enum HandType
    {
        Five = 5,
        Four = 4,
        Full = 3,
        Three = 2,
        TwoPair = 1,
        OnePair = 0,
        HighCard = -1,
    }

    record Bid(List<int> Hand, int Val) : IComparable<Bid>, IComparable
    {
        int J = 11;
        public Bid FindBest()
        {
            var ars = new List<List<int>>()
            {
                new()
            };
            foreach (var i in Hand)
            {
                if (i != J)
                {
                    foreach (var ar in ars)
                    {
                        ar.Add(i);
                    }
                }
                else
                {
                    var next = new List<List<int>>();
                    foreach (var x in new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 13, 14 })
                    {
                        foreach (var ar in ars)
                        {
                            next.Add(ar.Append(x).ToList());
                        }
                    }
                    ars = next;
                }
            }
            var possible = ars.Select(x => new Bid(x, Val));
            return possible.OrderByDescending(x => x.Type()).First();
        }

        public override string ToString()
        {
            var toLetter = (int x) => x switch
            {
                14 => 'A',
                13 => 'K',
                12 => 'Q',
                11 => 'J',
                10 => 'T',
                _ => $"{x}"[0]
            };
            return $"{string.Join("", Hand.Select(toLetter))} {Val} ({Type()})";
        }
        HandType? _type = null;
        public HandType Type()
        {
            if (JLow && Hand.Contains(J))
            {
                _type ??= FindBest().Type();
                return _type.Value;
            }
            var highest = Hand.GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Count())
                .First();
            var secondHighest = Hand.GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Count())
                .Skip(1)
                .FirstOrDefault();
            return highest switch
            {
                5 => HandType.Five,
                4 => HandType.Four,
                3 when secondHighest == 2 => HandType.Full,
                3 when secondHighest == 1 => HandType.Three,
                2 => secondHighest == highest ? HandType.TwoPair : HandType.OnePair,
                _ => HandType.HighCard
            };
        }

        public int CompareTo(Bid? other)
        {
            return CompareBid(this, other!);
        }

        public int CompareTo(object? obj)
        {
            if (obj is Bid b) CompareTo(b);
            return 0;
        }
    }

    Bid ParseLine(string line) =>
        new(
            line.Split(" ")[0].Select(c => "0123456789TJQKA".IndexOf(c)).ToList(),
            int.Parse(line.Split(" ")[1]));

    List<Bid> ParseInput(string input) =>
        input.Lines().Select(ParseLine).ToList();

    static int CompareBid(Bid left, Bid right)
    {
        if (left.Type() != right.Type())
            return left.Type().CompareTo(right.Type());
        return left.Hand
            .Select(x => JLow && x == 11 ? -1 : x).Zip(
                right.Hand.Select(x => JLow && x == 11 ? -1 : x))
            .FirstOrDefault(x => x.First != x.Second)
            .Pipe(x =>
                x.First.CompareTo(x.Second));
    }

    public override string SolveA(string input)
    {
        JLow = false;
        var parsed = input.Pipe(ParseInput).ToList();
        parsed.Sort();
        var d = parsed.ToDictionary(x => x.Val);
        return parsed.Select((x, i) => x.Val * (i + 1)).Sum().ToString();
    }

    public override string SolveB(string input)
    {
        JLow = true;
        var parsed = input.Pipe(ParseInput)
            .ToList();
        parsed.Sort();
        var d = parsed.ToDictionary(x => x.Val);
        return parsed.Select((x, i) => x.Val * (i + 1)).Sum().ToString();
    }

    public Day07()
    {
        Tests = new()
        {
            new("a", Sample, "6440", SolveA),
            new("b", Sample, "5905", SolveB)
        };
    }

    string Sample = """
    32T3K 765
    T55J5 684
    KK677 28
    KTJJT 220
    QQQJA 483
    """;
}