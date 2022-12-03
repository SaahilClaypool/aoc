using SharpResult.FunctionalExtensions;

namespace Aoc.Solutions;
public class Day01 : Day
{
    IEnumerable<(int prev, int next)> Pairs(List<int> items) =>
        Enumerable.Range(1, items.Count - 1).Select(i =>
            (items[i - 1], items[i]));

    IEnumerable<(int a, int b, int c)> Triplets(List<int> items) =>
        Enumerable.Range(2, items.Count - 2).Select(i =>
            (items[i - 2], items[i - 1], items[i]));

    public override string SolveA(string input)
    {
        var pairs = input.Split("\n").Select(int.Parse).ToList().Then(Pairs);
        return pairs.Select(pair => pair.next > pair.prev ? 1 : 0).Sum().ToString();
    }

    public override string SolveB(string input)
    {
        var triplets = input.Split("\n").Select(int.Parse).ToList().Then(Triplets);
        var tripletAveraged = triplets.Select(t => t.a + t.b + t.c).ToList();
        var averagedPairs = Pairs(tripletAveraged);
        return averagedPairs.Select(pair => pair.next > pair.prev ? 1 : 0).Sum().ToString();
    }


    public Day01()
    {
        Tests = new()
        {

        };
    }
}