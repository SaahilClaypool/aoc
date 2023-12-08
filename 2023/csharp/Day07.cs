using System.Numerics;

namespace Aoc.Solutions.Y2023;

public class Day07 : Day
{
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
        public HandType Type()
        {
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
        return left.Hand.Zip(right.Hand).FirstOrDefault(x => x.First != x.Second)
            .Pipe(x => x.First.CompareTo(x.Second));
    }

    public override string SolveA(string input)
    {
        var parsed = input.Pipe(ParseInput).ToList();
        parsed.Sort();
        var d = parsed.ToDictionary(x => x.Val);
        return parsed.Select((x, i) => x.Val * (i + 1)).Sum().ToString();
    }

    public override string SolveB(string input)
    {
        throw new NotImplementedException();
    }

    public Day07()
    {
        Tests = new()
        {
            new("a", Sample, "6440", SolveA)
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