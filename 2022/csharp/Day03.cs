namespace Aoc2022;


public class Day03 : Day
{
    static int Score(char c) => 1 + (char.IsUpper(c) ? (c + 26 - 'A') : c - 'a');

    public override string SolveA(string input)
    {
        var score = input.Split('\n').Select(line =>
        {
            var half = line.Length / 2;
            var (left, right) = (line[..half], line[half..]);
            return left.Intersect(right).First();
        })
        .Select(Score)
        .Sum();
        return score.ToString();
    }

    public override string SolveB(string input)
    {
        var groups = input.Split('\n').Chunk(3);
        var badges = groups.Select(group => group[0].Intersect(group[1]).Intersect(group[2]).First());
        return badges.Select(Score).Sum().ToString();
    }

    public Day03()
    {
        Tests = new()
        {
            new("Score", "A", "27", s => Score(s[0]).ToString()),
            new(
                "A",
                """
                vJrwpWtwJgWrhcsFMMfFFhFp
                jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
                PmmdzqPrVvPwwTWBwg
                wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
                ttgJtRGJQctTZtZT
                CrZsJsPPZsGzwwsLwLmpwMDw
                """,
                "157",
                SolveA),
            new(
                "B",
                """
                vJrwpWtwJgWrhcsFMMfFFhFp
                jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
                PmmdzqPrVvPwwTWBwg
                wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
                ttgJtRGJQctTZtZT
                CrZsJsPPZsGzwwsLwLmpwMDw
                """,
                "70",
                SolveB
            )
        };
    }
}