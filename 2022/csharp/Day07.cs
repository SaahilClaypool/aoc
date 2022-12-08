namespace Aoc2022;


public partial class Day07 : Day
{
    record CommandOutput(string Command, List<FD> FDs);
    public abstract record FD
    {
        public string Name { get; set; } = string.Empty;
        public record Dir() : FD()
        {
            public List<FD> Items = new();
            public long? Size = null;
            public override string ToString() =>
                $"""
                Dir: {Name} - {string.Join(", ", Items.Select(i => i.Name))}
                """;
        };
        public record File(long Size) : FD();
    }


    static IEnumerable<CommandOutput> Parse(string input)
    {
        CommandOutput? currentCommand = null;
        foreach (var line in input.Split('\n'))
        {
            if (line.StartsWith("$"))
            {
                if (currentCommand != null)
                {
                    yield return currentCommand;
                }
                currentCommand = new(line, new());
                continue;
            }
            Log($"Adding {line}");
            currentCommand!.FDs.Add(
                line.StartsWith("dir") ?
                    ParseDir(line) : ParseFile(line)
            );
        }
        if (currentCommand != null)
        {
            yield return currentCommand;
        }

        FD.Dir ParseDir(string line) => new() { Name = line.Split(" ")[1] };
        FD.File ParseFile(string line) => new(long.Parse(line.Split(" ")[0])) { Name = line.Split(" ")[1] };
    }

    public override string SolveA(string input)
    {
        var commands = Parse(input).ToList();
        var dir = ToTree(commands);
        Log($"Items: {dir.Items.Count}");
        Size(dir);
        var bigDirs = dir.Items.SelectMany(i => i switch
        {
            FD.Dir d => d.Items.Append(d),
            FD.File f => new List<FD> { },
            _ => throw new NotImplementedException()
        })
        .Cast<FD.Dir>().ToList();
        Log(bigDirs.Count.ToString());
        foreach (var d in bigDirs)
        {
            Log(d);
        }
        return bigDirs
        .Sum(f => f.Size)
        .ToString()!;
    }

    public long Size(FD.Dir dir)
    {
        if (dir.Size != null)
        {
            return dir.Size.Value;
        }
        return dir.Items.Select(i => i switch
        {
            FD.Dir d => Size(d),
            FD.File f => f.Size,
            _ => throw new NotImplementedException()
        }).Sum();
    }



    private static FD.Dir ToTree(IEnumerable<CommandOutput> commands)
    {
        var stack = new Stack<FD.Dir>();
        var dictFiles = new Dictionary<string, FD.Dir>();
        var root = new FD.Dir();
        stack.Push(root);
        foreach (var command in commands.Skip(1))
        {
            var currentDir = stack.Peek();
            Log(command.Command);
            if (command.Command.StartsWith("$ cd"))
            {
                var dir = command.Command.Split(" ")[2];
                if (dir == "..")
                {
                    stack.Pop();
                }
                else
                {
                }
            }
            else
            {
                foreach (var subFile in command.FDs)
                {
                    Log($"Adding {subFile}");
                    subFile.Name = currentDir.Name + "/" + subFile.Name;
                    currentDir.Items.Add(subFile);
                }
            }
        }
        return root;
    }

    public override string SolveB(string input)
    {
        throw new NotImplementedException();
    }

    public Day07()
    {
        Tests = new()
        {
            new("A",
            """
            $ cd /
            $ ls
            dir a
            14848514 b.txt
            8504156 c.dat
            dir d
            $ cd a
            $ ls
            dir e
            29116 f
            2557 g
            62596 h.lst
            $ cd e
            $ ls
            584 i
            $ cd ..
            $ cd ..
            $ cd d
            $ ls
            4060174 j
            8033020 d.log
            5626152 d.ext
            7214296 k
            """,
            "95437",
            SolveA)
        };
    }
}