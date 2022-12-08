namespace Aoc2022;


public partial class Day07 : Day
{
    public abstract class FD
    {
        public abstract long Size { get; }
        public FD(string name) => Name = name;
        public string Name { get; set; }
        public class File : FD
        {
            public override long Size { get; }
            public File(string name, long size) : base(name)
            {
                Name = name;
                Size = size;
            }
        }

        public class Dir : FD
        {
            public override long Size => Children.Sum(c => c.Size);

            public List<FD> Children { get; set; } = new();
            public Dir(string name) : base(name)
            {
                Name = name;
            }
        }
    }

    public FD.Dir FindRoot(string input)
    {
        var lines = input.Split('\n');

        var root = new FD.Dir("/");
        var stack = new Stack<FD.Dir>();
        stack.Push(root);
        for (var i = 1; i < lines.Length; i++)
        {
            var command = lines[i];
            Log(command);
            if (command == "$ cd ..")
            {
                stack.Pop();
                Log($"Moved .. to " + stack.Peek().Name);
            }
            else if (command.StartsWith("$ cd"))
            {
                var dir = command.Split(" ")[2];
                var child = stack.Peek().Children.FirstOrDefault(c => c.Name == dir);
                if (child is null)
                {
                    child = new FD.Dir(dir);
                    stack.Peek().Children.Add(child);
                }
                stack.Push((FD.Dir)child);
                Log($"CD to " + stack.Peek().Name);
            }
            else if (command.StartsWith("$ ls"))
            {
                i++;
                while (i < lines.Length && !lines[i].StartsWith("$"))
                {
                    var curLine = lines[i];
                    if (curLine.StartsWith("dir"))
                    {
                        var dir = new FD.Dir(curLine.Split(" ")[1]);
                        Log($"Adding dir {dir.Name}");
                        if (!stack.Peek().Children.Any(c => c.Name == dir.Name))
                        {
                            stack.Peek().Children.Add(dir);
                        }
                    }
                    else
                    {
                        var f = new FD.File(
                            curLine.Split(" ")[1],
                            long.Parse(curLine.Split(" ")[0]));
                        Log($"Adding file {f.Name} {f.Size}");
                        if (!stack.Peek().Children.Any(c => c.Name == f.Name))
                        {
                            stack.Peek().Children.Add(f);
                        }
                    }
                    i++;
                }
                i--;
            }
        }
        return root;
    }

    public override string SolveA(string input)
    {
        var root = FindRoot(input);
        var allDirs = Walk(root).Where(_ => _ is FD.Dir);
        foreach (var valid in allDirs)
        {
            Log($"all: {valid.GetType().Name} {valid.Name} {valid.Size}");
        }
        var validDirs = allDirs.Where(d => d.Size <= 100000);
        foreach (var valid in validDirs)
        {
            Log($"valid: {valid.GetType().Name} {valid.Name} {valid.Size}");
        }
        PP(root);
        return validDirs.Sum(d => d.Size).ToString();
    }

    IEnumerable<FD> Walk(FD fd)
    {
        if (fd is FD.File f)
        {
            yield return f;
        }
        else if (fd is FD.Dir d)
        {
            yield return d;
            foreach (var child in d.Children)
            {
                foreach (var nested in Walk(child))
                {
                    yield return nested;
                }
            }
        }
    }

    public void PP(FD fd, int indent = 0)
    {
        if (!IsTest)
        {
            return;
        }
        Console.Write(string.Join("", Enumerable.Range(0, indent).Select(_ => " ")));
        if (fd is FD.Dir d)
        {
            Console.WriteLine($"- {d.Name} (dir, size={d.Size})");
            foreach (var item in d.Children)
            {
                PP(item, indent + 2);
            }
        }
        else if (fd is FD.File f)
        {
            Console.WriteLine($"- {f.Name} (file, size={f.Size})");
        }
    }

    public override string SolveB(string input)
    {
        var root = FindRoot(input);
        var used = root.Size;
        var total = 70000000;
        var remaining = total - used;
        var target = 30000000;
        var toCut = target - remaining;
        var biggerThanRemaining = Walk(root).Where(f => f is FD.Dir).Where(f => f.Size >= toCut);
        return biggerThanRemaining.Min(f => f.Size).ToString();
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
                SolveA
            ),
            new("B",
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
                "24933642",
                SolveB
            )
        };
    }
}