global using SharpResult;
global using static SharpResult.Result;
global using static SharpResult.Option;
global using SharpResult.FunctionalExtensions;
global using static System.Linq.Enumerable;

namespace Aoc.Solutions.Day04;

record Pos(int Row, int Col, int Val)
{
    public bool Marked { get; set; }
}

record Board(List<List<Pos>> State)
{
    public bool HasWon { get; set; } = false;

    public override string ToString() =>
        State
            .Select(row => row.Select(pos => $"{(pos.Marked ? '[' : ' ')}{pos.Val}{(pos.Marked ? ']' : ' ')}").Join(" "))
            .Prepend("\n")
            .Join("\n");
    public bool IsWon()
    {
        foreach (var row in State)
        {
            if (row.All(p => p.Marked))
            {
                // Console.WriteLine($"{row.Join(" ")}");
                return true;
            }
        }
        foreach (var colI in Enumerable.Range(0, State[0].Count))
        {
            var col = State.Select(r => r[colI]);
            if (col.All(p => p.Marked))
            {
                // Console.WriteLine($"{colI}: {col.Join(" ")} {col.Count()}");
                return true;
            }
        }
        return false;
    }
    public int Score(int i) =>
        State.SelectMany(r => r).Where(pos => !pos.Marked).Select(pos => pos.Val).Sum() * i;
}

record GameState(
    Dictionary<int, List<(int Board, Pos Pos)>> Postings,
    List<Board> Boards
);

public class Day04 : Day
{
    static (List<int>, GameState) ParseInput(string input)
    {
        var lines = input.Split('\n');
        var bingoNumbers = lines.First().Split(',').Select(int.Parse).ToList();

        var boards = lines
            .Skip(2)
            .SplitAt(line => line.Trim().Length == 0)
            .Select(ParseBoard)
            .ToList();
        var postings = boards
            .SelectMany((board, boardIdx) =>
                board
                    .State
                    .SelectMany(row => row.Select(pos => (boardIdx, pos)))
            )
            .GroupBy(pos => pos.pos.Val)
            .ToDictionary(
                val => val.Key,
                val => val.ToList()
            );

        return (bingoNumbers, new GameState(postings, boards));
    }

    static Board ParseBoard(IEnumerable<string> boardStr)
    {
        var boardState = boardStr
            .Select((rowValues, row) =>
                rowValues
                    .Split(' ')
                    .Where(s => s.Length > 0)
                    .Select(int.Parse)
                    .Select((val, col) =>
                        new Pos(row, col, val))
                    .ToList())
            .ToList();
        return new Board(boardState);
    }

    Option<Board> Execute(int n, GameState state)
    {
        // Console.WriteLine($"Executing {n}");
        if (!state.Postings.TryGetValue(n, out var postings))
        {
            return None;
        }
        foreach (var posting in postings)
        {
            posting.Pos.Marked = true;
        }
        foreach (var board in state.Boards)
        {
            // Console.WriteLine(board);
            if (board.IsWon())
            {
                // Console.WriteLine("Won");
                return board;
            }
        }
        return None;
    }


    public override string SolveA(string input)
    {
        var (nums, boards) = ParseInput(input);

        foreach (var i in nums)
        {
            var winner = Execute(i, boards);
            if (winner.IsSome)
            {
                return winner.Unwrap().Score(i).ToString();
            }
        }

        return "no winner?";
    }

    public override string SolveB(string input)
    {
        var (nums, boards) = ParseInput(input);

        foreach (var i in nums)
        {
            Execute(i, boards);
            var notWonBoards = boards.Boards.Where(b => !b.HasWon).ToList();
            var winningBoards = notWonBoards.Where(b => b.IsWon()).ToList();
            if (notWonBoards.Count == 1 && winningBoards.Count == 1)
            {
                return winningBoards.First().Score(i).ToString();
            }
            else
            {
                foreach (var winning in winningBoards)
                {
                    winning.HasWon = true;
                }
            }
        }

        return "no winner?";
    }


    public Day04()
    {
        Tests = new()
        {
            new("A", @"7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

22 13 17 11  0
 8  2 23  4 24
21  9 14 16  7
 6 10  3 18  5
 1 12 20 15 19

 3 15  0  2 22
 9 18 13 17  5
19  8  7 25 23
20 11 10 24  4
14 21 16 12  6

14 21 17 24  4
10 16 15  9 19
18  8 23 26 20
22 11 13  6  5
 2  0 12  3  7", "4512", SolveA),
            new("B", @"7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

22 13 17 11  0
 8  2 23  4 24
21  9 14 16  7
 6 10  3 18  5
 1 12 20 15 19

 3 15  0  2 22
 9 18 13 17  5
19  8  7 25 23
20 11 10 24  4
14 21 16 12  6

14 21 17 24  4
10 16 15  9 19
18  8 23 26 20
22 11 13  6  5
 2  0 12  3  7", "1924", SolveB)
        };
    }
}
