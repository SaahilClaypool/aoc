namespace Aoc2022;


using static Day02.Move;
public class Day02 : Day
{
    public enum Move { Rock = 1, Paper = 2, Scissors = 3 };

    private static long ScoreDay((Move them, Move me) move)
    {
        var (them, me) = move;
        var win = them == Rock && me == Paper ||
                        them == Paper && me == Scissors ||
                        them == Scissors && me == Rock;
        var tie = them == me;
        var score = (long)me + (win ? 6 : tie ? 3 : 0);
        Log($"{them} {me} {score}");
        return score;
    }

    public override string SolveA(string input)
    {
        var moves = input.Split('\n').Select(line =>
       {
           var parts = line.Split(' ');
           var me = parts[1] switch { "X" => Rock, "Y" => Paper, "Z" => Scissors, _ => throw new Exception() };
           var them = parts[0] switch { "A" => Rock, "B" => Paper, "C" => Scissors, _ => throw new Exception() };
           return (them, me);
       }).ToList();

        return moves.Select(ScoreDay).Sum().ToString();
    }

    public override string SolveB(string input)
    {
        var moves = input.Split('\n').Select(line =>
        {
            var parts = line.Split(' ');
            var them = parts[0] switch { "A" => Rock, "B" => Paper, "C" => Scissors, _ => throw new Exception() };
            var me = parts[1] switch
            {
                // win
                "X" => them switch { Rock => Scissors, Paper => Rock, Scissors => Paper, _ => throw new NotImplementedException() },
                "Y" => them,
                "Z" => them switch { Rock => Paper, Paper => Scissors, Scissors => Rock, _ => throw new NotImplementedException() },
                _ => throw new NotImplementedException()
            };
            return (them, me);
        }).ToList();

        return moves.Select(ScoreDay).Sum().ToString();
    }

    public Day02()
    {
        Tests = new()
        {
            new(
                "A",
                """
                A Y
                B X
                C Z
                """,
                "15",
                SolveA
            )
        };
    }
}