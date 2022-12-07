namespace Aoc2022;


public partial class Day06 : Day
{
    private static string Solve(string input, int window)
    {
        for (var i = window; i <= input.Length; i++)
        {
            var chars = input[(i - window)..i].ToHashSet();
            if (chars.Count == window)
            {
                return i.ToString();
            }
        }

        throw new NotImplementedException();
    }
    public override string SolveA(string input) => Solve(input, 4);

    public override string SolveB(string input) => Solve(input, 14);

    public Day06()
    {
        Tests = new()
        {
            new("A1", "bvwbjplbgvbhsrlpgdmjqwftvncz", "5", SolveA),
            new("A2", "nppdvjthqldpwncqszvftbrmjlhg", "6", SolveA),
            new("A3", "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", "10", SolveA),
            new("B1", "mjqjpqmgbljsphdztnvjfqwrcgsmlb", "19", SolveB)
        };
    }
}