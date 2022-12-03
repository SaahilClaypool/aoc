namespace Aoc.Solutions.Day12;

public class Day12 : Day
{
    static bool IsSmall(string cave) => cave.ToLower() == cave;

    IEnumerable<List<string>> Paths(
        Dictionary<string, List<string>> connections,
        List<string> curPath,
        HashSet<string> triedPaths,
        bool doubleUsed = true)
    {
        var cur = curPath.Last();
        var visited = curPath.ToHashSet();
        var choices =
            (connections.TryGetValue(cur, out var c) ? c : new())
            .Where(c =>
                c != "start" &&
                (!(IsSmall(c) && visited.Contains(c)) ||
                    !doubleUsed)
            );

        Log($" {curPath.Join(",")}: From {cur} - {choices.ToJson(false)} - usedDouble = {doubleUsed} visited = {visited.ToJson(false)}");

        foreach (var choice in choices)
        {
            var withChoice = curPath.Append(choice).ToList();
            var pathName = withChoice.Join(",");
            if (triedPaths.Contains(pathName))
            {
                continue;
            }
            triedPaths.Add(pathName);
            if (choice == "end")
            {
                yield return withChoice;
            }
            else
            {
                var newDoubleUsed = doubleUsed || IsSmall(choice) && visited.Contains(choice);
                var subPaths = Paths(connections, withChoice, triedPaths, newDoubleUsed);
                foreach (var sub in subPaths)
                {
                    yield return sub;
                }
            }
        }
    }

    static Dictionary<string, List<string>> Parse(string input) =>
        input
        .Split('\n')
        .Select(s => s.Split('-'))
        .SelectMany(s => new List<(string, string)> { (s[0], s[1]), (s[1], s[0]) })
        .GroupBy(s => s.Item1)
        .ToDictionary(s => s.Key, s => s.Select(v => v.Item2).ToList());

    public override string SolveA(string input)
    {
        var conns = Parse(input);
        var allPaths = Paths(conns, new() { "start" }, new() { "start" });
        return allPaths.Count().ToString();
    }

    public override string SolveB(string input)
    {
        var conns = Parse(input);
        var allPaths = Paths(conns, new() { "start" }, new() { "start" }, false).ToList();
        return allPaths.Count.ToString();
    }

    public Day12()
    {
        Tests = new()
        {
            new("A", @"start-A
start-b
A-c
A-b
b-d
A-end
b-end", "10", SolveA),
            new("A2", @"fs-end
he-DX
fs-he
start-DX
pj-DX
end-zg
zg-sl
zg-pj
pj-he
RW-he
fs-DX
pj-RW
zg-RW
start-pj
he-WI
zg-he
pj-fs
start-RW", "226", SolveA),
            new("B", @"start-A
start-b
A-c
A-b
b-d
A-end
b-end", "36", SolveB)
        };
    }
}