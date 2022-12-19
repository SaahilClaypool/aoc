using System.Collections.Immutable;

namespace Aoc2022;


public partial class Day16 : Day
{
    record Valve(string Name, int FlowRate, List<string> Tunnels);
    record OpenEvent(Valve Valve, int Minute);
    record State(List<OpenEvent> Events, int Minute)
    {
        int? cachedScore = null;
        string? key = null;
        public string Key
        {
            get
            {
                key ??= string.Join(".", Open.OrderBy(o => o));
                return key;
            }
        }
        public int Score
        {
            get
            {
                cachedScore ??= Events.Sum(e => e.Minute * e.Valve.FlowRate);
                return cachedScore.Value;
            }
        }

        HashSet<string>? _open = null!;

        public HashSet<string> Open
        {
            get
            {
                _open ??= Events.Select(e => e.Valve.Name).ToHashSet();
                return _open;
            }
        }

    }


    public override string SolveA(string input)
    {
        var tunnels = Parse(input);
        var distances = DistanceToHighFlow(tunnels);

        var pq = new Queue<State>();
        pq.Enqueue(new(new(), 30));

        var allOrders = new List<State>();
        var best = pq.Peek();
        while (pq.TryDequeue(out var curState))
        {
            if (curState.Minute <= 0)
            {
                continue;
            }
            if (best.Score < curState.Score)
            {
                best = curState;
            }
            var currentSpot = curState.Events.LastOrDefault()?.Valve?.Name ?? "AA";
            var curOpen = curState.Events.Select(e => e.Valve.Name).ToHashSet();
            foreach (var (nextOpen, distance) in distances[currentSpot])
            {
                if (curOpen.Count == distances["AA"].Count)
                {
                    allOrders.Add(curState);
                    continue;
                }
                if (curOpen.Contains(nextOpen))
                {
                    continue;
                }
                var nextState = new State(
                    curState
                        .Events
                        .Append(new(tunnels[nextOpen], curState.Minute - distance - 1))
                        .ToList(),
                    curState.Minute - distance - 1);
                pq.Enqueue(nextState);
            }
        }

        return best.Score.ToString();
    }



    public override string SolveB(string input)
    {
        var tunnels = Parse(input);
        var distances = DistanceToHighFlow(tunnels);

        var pq = new Queue<State>();
        pq.Enqueue(new(new(), 26));

        var allOrders = new List<State>();
        var best = pq.Peek();
        while (pq.TryDequeue(out var curState))
        {
            allOrders.Add(curState);
            if (curState.Minute <= 0)
            {
                continue;
            }
            if (best.Score < curState.Score)
            {
                best = curState;
            }
            var currentSpot = curState.Events.LastOrDefault()?.Valve?.Name ?? "AA";
            var curOpen = curState.Events.Select(e => e.Valve.Name).ToHashSet();
            foreach (var (nextOpen, distance) in distances[currentSpot])
            {
                if (curOpen.Count == distances["AA"].Count)
                {
                    continue;
                }
                if (curOpen.Contains(nextOpen))
                {
                    continue;
                }
                var nextState = new State(
                    curState
                        .Events
                        .Append(new(tunnels[nextOpen], curState.Minute - distance - 1))
                        .ToList(),
                    curState.Minute - distance - 1);
                pq.Enqueue(nextState);
            }
        }

        allOrders = allOrders.OrderByDescending(o => o.Score).DistinctBy(o => o.Key).ToList();

        Console.WriteLine($"Comparing {allOrders.Count * allOrders.Count}");

        var bestPairScore = 0;
        for (var i = 0; i < allOrders.Count; i++)
        {
            var order = allOrders[i];
            for (var j = 0; j < i; j++)
            {
                var elephantOrder = allOrders[j];
                if (elephantOrder.Open.Any(order.Open.Contains))
                {
                    continue;
                }
                var pair = (order, elephantOrder with
                {
                    Events = elephantOrder.Events.ToList(),
                });
                var pairScore = pair.order.Score + pair.Item2.Score;
                if (pairScore > bestPairScore)
                {
                    bestPairScore = pairScore;
                }
            }
        }

        return bestPairScore.ToString();
    }

    public Day16()
    {
        Tests = new()
        {
            new("A",
            """
            Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
            Valve BB has flow rate=13; tunnels lead to valves CC, AA
            Valve CC has flow rate=2; tunnels lead to valves DD, BB
            Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
            Valve EE has flow rate=3; tunnels lead to valves FF, DD
            Valve FF has flow rate=0; tunnels lead to valves EE, GG
            Valve GG has flow rate=0; tunnels lead to valves FF, HH
            Valve HH has flow rate=22; tunnel leads to valve GG
            Valve II has flow rate=0; tunnels lead to valves AA, JJ
            Valve JJ has flow rate=21; tunnel leads to valve II
            """,
            "1651",
            SolveA),
            new("B",
            """
            Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
            Valve BB has flow rate=13; tunnels lead to valves CC, AA
            Valve CC has flow rate=2; tunnels lead to valves DD, BB
            Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
            Valve EE has flow rate=3; tunnels lead to valves FF, DD
            Valve FF has flow rate=0; tunnels lead to valves EE, GG
            Valve GG has flow rate=0; tunnels lead to valves FF, HH
            Valve HH has flow rate=22; tunnel leads to valve GG
            Valve II has flow rate=0; tunnels lead to valves AA, JJ
            Valve JJ has flow rate=21; tunnel leads to valve II
            """,
            "1707",
            SolveB),
        };
    }

    [GeneratedRegex("^Valve (?<name>.*) has flow rate=(?<rate>\\d+); tunnels? leads? to valves? (?<tunnels>.*)$")]
    private static partial Regex TunnelRegex();

    Dictionary<string, Valve> Parse(string input) =>
        input.Lines()
            .Select(ParseLine)
            .ToDictionary(t => t.Name);

    static Valve ParseLine(string input)
    {
        var re = TunnelRegex();
        var match = re.Match(input);
        return new(match.Groups["name"].Value.Trim(), int.Parse(match.Groups["rate"].Value),
            match.Groups["tunnels"].Value.Split(",").Select(v => v.Trim()).ToList());
    }

    Dictionary<string, Dictionary<string, int>> DistanceToHighFlow(Dictionary<string, Valve> valves)
    {
        var targetTunnels = valves.Values.Where(v => v.FlowRate > 0).ToDictionary(v => v.Name);
        var dict = new Dictionary<string, Dictionary<string, int>>();

        foreach (var (name, valve) in valves)
        {
            if (!dict.TryGetValue(name, out var entry))
            {
                entry = new();
                dict[name] = entry;
            }
            foreach (var connection in valve.Tunnels)
            {
                entry[connection] = 1;
            }
        }

        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var (name, valve) in valves)
            {
                var currentReachable = dict[name];
                foreach (var connection in currentReachable.Keys.ToList())
                {
                    var currentCost = currentReachable[connection];
                    var nextReachable = dict[connection];
                    foreach (var (nextConnName, cost) in nextReachable)
                    {
                        var totalCost = cost + currentCost;
                        if (!currentReachable.ContainsKey(nextConnName) || currentReachable[nextConnName] > totalCost)
                        {
                            currentReachable[nextConnName] = totalCost;
                            changed = true;
                        }
                    }
                }
            }
        }

        return dict
            .Select(v =>
                (v.Key, Value: v.Value.Where(v => targetTunnels.ContainsKey(v.Key)).ToDictionary(v => v.Key, v => v.Value)))
            .ToDictionary(v => v.Key, v => v.Value);
    }
}