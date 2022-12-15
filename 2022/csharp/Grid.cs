namespace Aoc2022.GridType;

public record Grid<T>(List<List<T>> Data, bool Diagonal = false)
{
    public int Rows => Data.Count;
    public int Cols => Data[0].Count;

    public T this[Pos index]
    {
        get => Data[index.Row][index.Col];
        set => Data[index.Row][index.Col] = value;
    }

    public bool InBounds(Pos pos) => pos.Row >= 0 && pos.Row < Rows && pos.Col >= 0 && pos.Col < Cols;

    public IEnumerable<Pos> Surrounding(Pos current, bool? diagonal = null)
    {
        diagonal ??= Diagonal;
        for (var r = current.Row - 1; r <= current.Row + 1; r++)
            for (var c = current.Col - 1; c <= current.Col + 1; c++)
                if ((r == current.Row || c == current.Col || diagonal == true) &&
                    !(r == current.Row && c == current.Col) &&
                    c >= 0 && c < Cols && r >= 0 && r < Rows)
                    yield return new(r, c);
    }


}

public record Pos(int Row, int Col);