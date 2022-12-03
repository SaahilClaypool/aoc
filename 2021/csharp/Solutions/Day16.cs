using static Aoc.Solutions.Day17.Packet;

namespace Aoc.Solutions.Day17;

public class Day16 : Day
{
    static Parser ParseFromString(string input) => new(new Queue<char>(input.SelectMany(HexToInt)));

    public override string SolveA(string input)
    {
        Log(input);
        var p = ParseFromString(input);
        Log(p.Packet.Join(""));
        var packet = p.ParsePacket();
        Log(packet.ToString());
        return packet.Packets().Select(p => (decimal)p.Header.Version).Sum().ToString()!;
    }

    public override string SolveB(string input)
    {
        Log(input);
        var p = ParseFromString(input);
        Log(p.Packet.Join(""));
        var packet = p.ParsePacket();
        Log(packet.ToString());
        return packet.GetValue().ToString();
    }

    public Day16()
    {
        Tests = new()
        {
            new("A", @"D2FE28", @"2021", p => (ParseFromString(p).ParsePacket() as Literal)?.Value.ToString()!),
            new("A1", @"8A004A801A8002F478", "16", SolveA),
            new("B", @"9C0141080250320F1802104A08", "1", SolveB),
            new("B2", @"CE00C43D881120", "9", SolveB)
        };
    }

    static char[] HexToInt(char hex) => hex switch
    {
        '0' => new char[] { '0', '0', '0', '0' },
        '1' => new char[] { '0', '0', '0', '1' },
        '2' => new char[] { '0', '0', '1', '0' },
        '3' => new char[] { '0', '0', '1', '1' },
        '4' => new char[] { '0', '1', '0', '0' },
        '5' => new char[] { '0', '1', '0', '1' },
        '6' => new char[] { '0', '1', '1', '0' },
        '7' => new char[] { '0', '1', '1', '1' },
        '8' => new char[] { '1', '0', '0', '0' },
        '9' => new char[] { '1', '0', '0', '1' },
        'A' => new char[] { '1', '0', '1', '0' },
        'B' => new char[] { '1', '0', '1', '1' },
        'C' => new char[] { '1', '1', '0', '0' },
        'D' => new char[] { '1', '1', '0', '1' },
        'E' => new char[] { '1', '1', '1', '0' },
        'F' => new char[] { '1', '1', '1', '1' },
        _ => throw new Exception()
    };

}

record Parser(Queue<char> Packet)
{
    private Literal ParseLiteral(Header header)
    {
        Log($"Parsing literal");
        List<char> allBits = new();
        while (true)
        {
            var cur = Consume(5);
            Log($"literal cur: {cur.Join("")}");
            allBits.AddRange(cur.Skip(1));
            if (cur[0] != '1')
            {
                break;
            }
        }
        var val = BinToInt(new string(allBits.ToArray()));
        Log($"Value: {val}");
        return new(header, val);
    }

    private Operator ParseOperator(Header header)
    {
        var lengthType = Consume(1).First();
        if (lengthType == '0')
        {
            var length = ConsumeInt(15);
            Log($"Parsing operator with length type {lengthType} -> {length} bits");
            var totalToConsume = _consumed + length;
            var subPackets = new List<Packet>();
            Log($"at {_consumed}, target {totalToConsume}");
            while (_consumed < totalToConsume)
            {
                subPackets.Add(ParsePacket());
                Log($"consumed {_consumed} of {totalToConsume}");
            }
            return new(header, subPackets);
        }
        else
        {
            var packets = ConsumeInt(11);
            Log($"Parsing operator with length type {lengthType} -> {packets} packets");
            var subPackets = new List<Packet>((int)packets);
            foreach (var _ in Range(0, (int)packets))
            {
                subPackets.Add(ParsePacket());
            }
            return new(header, subPackets);
        }
    }

    private ulong _consumed;
    static ulong BinToInt(string bin) => Convert.ToUInt64(bin, 2);
    ulong ConsumeInt(ulong n) => BinToInt(new string(Consume(n)));

    char[] Consume(ulong n)
    {
        Log($"Consuming {n}");
        _consumed += n;
        return Packet.Dequeue((int)n);
    }

    public Packet ParsePacket()
    {
        var header = new Header(ConsumeInt(3), ConsumeInt(3));
        Log(header.ToString());
        return header.TypeId switch
        {
            4 => ParseLiteral(header),
            _ => ParseOperator(header)
        };
    }
}

public record Header(ulong Version, ulong TypeId);
public abstract record Packet(Header Header)
{
    public abstract ulong GetValue();
    public virtual ulong PacketCount => 1;
    public virtual IEnumerable<Packet> Packets() => new List<Packet> { this };
    public record Literal(Header Header, ulong Value) : Packet(Header)
    {
        public override ulong GetValue() => Value;
    }
    public record Operator(Header Header, List<Packet> SubPackets) : Packet(Header)
    {
        public override ulong GetValue()
        {
            var values = SubPackets.Select(v => v.GetValue()).ToList();
            return Header.TypeId switch
            {
                0 => Sum(values),
                1 => Product(values),
                2 => Minimum(values),
                3 => Maximum(values),
                5 => Greater(values),
                6 => Lesser(values),
                7 => Equal(values),
                _ => throw new NotImplementedException()
            };
        }

        public override IEnumerable<Packet> Packets() =>
            new List<Packet> { this }.Concat(SubPackets.SelectMany(p => p.Packets()));
        public override string ToString()
        {
            return $"{nameof(Operator)} {{ {Header}, [\n\t{SubPackets.Join("\n\t")}] }}";
        }

        private static ulong Equal(List<ulong> values) =>
            values[0] == values[1] ? 1UL : 0UL;

        private static ulong Lesser(List<ulong> values) =>
            values[0] < values[1] ? 1UL : 0UL;

        private static ulong Greater(List<ulong> values) =>
            values[0] > values[1] ? 1UL : 0UL;


        public static ulong Product(IEnumerable<ulong> items) =>
            items.Aggregate(1UL, (xs, v) => v * xs);

        public static ulong Sum(IEnumerable<ulong> items) =>
            items.Aggregate(0UL, (xs, v) => v + xs);

        public static ulong Minimum(IEnumerable<ulong> items) =>
            items.Aggregate(ulong.MaxValue, (xs, v) => v < xs ? v : xs);

        public static ulong Maximum(IEnumerable<ulong> items) =>
            items.Aggregate(ulong.MinValue, (xs, v) => v > xs ? v : xs);
    }
}