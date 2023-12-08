using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Aoc.Runner
{
    public abstract class Day
    {
        public bool IsTest { get; set; } = false;
        public virtual List<Test> Tests { get; set; } = new();
        public virtual uint Number()
        {
            try
            {
                return uint.Parse(NumberString());
            }
            catch
            {
                return 999;
            }
        }
        public virtual string NumberString()
        {
            var className = GetType().Name;
            var numberString = className[(className.IndexOf("Day") + 3)..].TrimStart('_');
            return numberString;
        }
        public virtual string GetInput()
        {
            var path = Path.Combine("../inputs", $"Day_{NumberString()}.txt");
            return File.ReadAllText(path);
        }

        public virtual string GetInput(string suffix)
        {
            var path = Path.Combine("../inputs", $"Day_{NumberString()}_{suffix}.txt");
            return File.ReadAllText(path);
        }

        public virtual string SolveA() => SolveA(GetInput());
        public abstract string SolveA(string input);
        public virtual string SolveB() => SolveB(GetInput());
        public abstract string SolveB(string input);

    }
    
    public static class LogHelpers
    {
        private static bool IsTest = false;
        public static void SetTest(bool enable = true) => IsTest = enable;

        public static T Log<T>(T t)
        {
            if (IsTest)
            {
                Console.WriteLine(t?.ToString());
            }
            return t;
        }

        public static T Log<T>(this T t, [CallerArgumentExpression(nameof(t))] string? name = null, bool ignoreTest = false)
        {
            if (IsTest || ignoreTest)
            {
                Console.WriteLine($"{name} - {t?.ToString()}");
            }
            return t;
        }
    }


    public class Test
    {
        public string Output { get; set; } = "NONE";
        public string Name { get; }
        public string Input { get; }
        public string ExpectedOutput { get; }
        public Func<string, string> Solve { get; }

        public Test(string name, string input, string expectedOutput, Func<string, string> solve)
        {
            Name = name;
            Input = input;
            ExpectedOutput = expectedOutput;
            Solve = solve;
        }

        public bool Run()
        {
            try
            {
                Output = Solve.Invoke(Input);
            }
            catch (Exception ex)
            {
                LogHelpers.Log($"Failed test {Name} with {ex.Message}\n({ex.StackTrace})");
                return false;
            }
            return Output == ExpectedOutput;
        }
    }
}