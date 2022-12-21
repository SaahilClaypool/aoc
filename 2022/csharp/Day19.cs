using static Aoc.Runner.Helpers;

namespace Aoc2022;


public partial class Day19 : Day
{
    public override string SolveA(string input)
    {
        var bps = Parse(input);
        Log(bps.ToJson(true));
        var states = bps.Select(bp => new State(new(), bp, new() { [Resource.Ore] = 1 }, 0)).ToList();

        var bestStates = new List<State>();
        foreach (var state in states)
        {
            foreach (var _ in Enumerable.Range(0, 50_000))
            {
                var currentStates = new[] { state }.ToList();
                foreach (var m in Enumerable.Range(0, 24))
                {
                    currentStates = currentStates.SelectMany(NextStates).ToList();
                }
                bestStates.AddRange(currentStates);
            }
        }

        var best = bestStates.Max(s => s.Resources[Resource.Geode]);
        return best.ToString();
    }

    /// <summary>
    /// only valid if the number of bots at current time is less than the max needed for any bot
    /// </summary>
    private bool IsValid(State arg)
    {
        var maxCosts = arg.BP.Bots
            .SelectMany(b => b.Value.Cost).GroupBy(c => c.Key)
            .ToDictionary(g => g.Key, g => g.Select(_ => _.Value).Max())
            .ToDefaultDict(() => 1000);
        
        return arg.Bots.All(b => b.Key == Resource.Geode || b.Value <= maxCosts[b.Key] + 1);
    }

    public override string SolveB(string input)
    {
        throw new NotImplementedException();
    }

    DefaultDictionary<Resource, int> Add(DefaultDictionary<Resource, int> init, DefaultDictionary<Resource, int> botOutputs)
    {
        var newOutput = init.ToDefaultDict();
        foreach (var (resource, value) in botOutputs)
        {
            newOutput[resource] += value;
        }
        return newOutput;
    }

    DefaultDictionary<Resource, int> Subtract(DefaultDictionary<Resource, int> init, DefaultDictionary<Resource, int> cost)
    {
        var newOutput = init.ToDefaultDict();
        foreach (var (resource, value) in cost)
        {
            newOutput[resource] -= value;
        }
        return newOutput;
    }

    DefaultDictionary<Resource, int> Clone(DefaultDictionary<Resource, int> init)
    {
        var newOutput = init.ToDefaultDict();
        return newOutput;
    }

    IEnumerable<State> NextStates(State s)
    {
        // bots at start of round
        var resourcesToAdd = Clone(s.Bots);

        // build if possible
        List<State> viable = new();

        if (!AddState(s, viable, Resource.Geode, s.BP.Bots[Resource.Geode]))
        {
            foreach (var (type, robot) in s.BP.Bots)
            {
                AddState(s, viable, type, robot);
            }
            viable.Add(s with { Minute = s.Minute + 1});
        }
        


        return viable
            .OrderByDescending(_ => Random.Shared.Next())
            .Take(1)
            .Select(v =>
                v with { Resources = Add(v.Resources, resourcesToAdd) }
            );
    }

    private bool AddState(State s, List<State> viable, Resource type, Robot robot)
    {
        var newResources = robot.Cost.ToDictionary(
            c => c.Key,
            c => s.Resources[c.Key] - c.Value
        ).ToDefaultDict();
        if (newResources.Values.All(r => r >= 0))
        {
            var newBots = Clone(s.Bots);
            newBots[type] += 1;
            viable.Add(s with { Bots = newBots, Resources = newResources, Minute = s.Minute + 1 });
            return true;
        }
        return false;
    }

    record State(DefaultDictionary<Resource, int> Resources, BluePrint BP, DefaultDictionary<Resource, int> Bots, int Minute);
    enum Resource { Ore, Clay, Obsidian, Geode };
    record Robot(DefaultDictionary<Resource, int> Cost);
    record BluePrint(int Num, Dictionary<Resource, Robot> Bots);

    List<BluePrint> Parse(string input)
    {
        return input.Lines()
            .Select((line, idx) =>
            {
                var plans = line.Split(":")[1];
                var bots = plans.Split(".");
                return new BluePrint(idx + 1,
                    new()
                    {
                        [Resource.Ore] = ParseBot(bots[0]),
                        [Resource.Clay] = ParseBot(bots[1]),
                        [Resource.Obsidian] = ParseBot(bots[2]),
                        [Resource.Geode] = ParseBot(bots[3]),
                    }
                );
            }).ToList();
    }

    private Robot ParseBot(string v)
    {
        var re = BotRegex();
        var costs = re.Matches(v).ToDictionary(
            match => Enum.Parse<Resource>(match.Groups["type"].Value, true),
            match => int.Parse(match.Groups["cost"].Value)
        ).ToDefaultDict();
        return new(
            Cost: costs
        );
    }

    public Day19()
    {
        Tests = new()
        {
            new("A",
            """
            Blueprint 1: Each ore robot costs 4 ore.  Each clay robot costs 2 ore.  Each obsidian robot costs 3 ore and 14 clay.  Each geode robot costs 2 ore and 7 obsidian.
            Blueprint 2: Each ore robot costs 2 ore.  Each clay robot costs 3 ore.  Each obsidian robot costs 3 ore and 8 clay.  Each geode robot costs 3 ore and 12 obsidian.
            """,
            "33",
            SolveA),
            new("B",
            """
            1
            """,
            "",
            SolveB)
        };
    }

    [GeneratedRegex("(?<cost>\\d+) (?<type>\\w+)")]
    private static partial Regex BotRegex();
}