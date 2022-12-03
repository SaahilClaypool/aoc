using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Spectre.Console;

namespace Aoc.Runner
{
    /// Main entrypoint class to run solutions
    public class AocRunner
    {

        public static void Run(string[] args) =>
            Cli.Run(args);

        public static IEnumerable<Day> Days()
        {
            var assembly = Assembly.GetEntryAssembly()!;
            return assembly.GetTypes()
                .Where(type => typeof(Day).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .Select(type => (Day)Activator.CreateInstance(type)!)
                .OrderBy(day => day.Number() > 25 ? -1 : (int)day.Number())
                .ToList();
        }

        public static bool TestDay(Day day)
        {
            day.IsTest = true;
            LogHelpers.SetTest();
            var failedTests = day.Tests.Select(test =>
            {
                var (time, passed) = TimeIt(() => test.Run());
                return (test, failed: !passed, time);
            }).ToList();
            var color = failedTests.Any(res => res.failed) ? "red" : "green";
            var content = new Table()
                .AddColumn($"[{color} bold underline]{day.GetType().Name}[/]")
                .AddColumn("name")
                .AddColumn("expected")
                .AddColumn("actual")
                .AddColumn("time");
            foreach (var (test, failed, time) in failedTests)
            {
                content.AddRow(failed ? "[red underline]Fail[/]" : "[green]Pass[/]", test.Name,
                    test.ExpectedOutput.EscapeMarkup(),
                    test.Output.EscapeMarkup(),
                    $"{time.Milliseconds}ms");
            }
            AnsiConsole.Write(content);
            return !failedTests.Any();
        }

        public static bool RunDay(Day day)
        {
            var content = new Table()
                .AddColumn($"[bold underline]{day.GetType().Name}[/]")
                .AddColumn("result")
                .AddColumn("time");
            try
            {
                var a = "";
                var elapsed = TimeIt(() =>
                {
                    a = day.SolveA();
                });
                content.AddRow("A", a, $"{elapsed.Milliseconds + elapsed.Seconds * 1000}ms");
            }
            catch (NotImplementedException)
            {
                content.AddRow("A", $"{day.NumberString()} is not implmented!", double.NaN.ToString());
            }

            try
            {
                var b = "";
                var elapsed = TimeIt(() =>
                {
                    b = day.SolveB();
                });
                content.AddRow("B", b, $"{elapsed.Milliseconds + elapsed.Seconds * 1000}ms");
            }
            catch (NotImplementedException)
            {
                content.AddRow("A", $"{day.NumberString()} is not implmented!", double.NaN.ToString());
            }
            AnsiConsole.Write(content);
            return true;
        }

        public static int TestAll() =>
            Days().Where(day => TestDay(day)).Count();

        public static bool TestLast() =>
            !TestDay(Days().Last());

        public static bool TestOne(string day) =>
            !TestDay(Find(day));

        public static int RunAll() =>
            Days().Where(day => RunDay(day)).Count();

        public static bool RunLast() =>
            !RunDay(Days().Last());

        public static bool RunOne(string day) =>
            !RunDay(Find(day));

        private static Day Find(string day) =>
            Days().Where(dayClass => dayClass.GetType().Name == day).LastOrDefault() ?? throw new NoDayFound();

        static TimeSpan TimeIt(Action blockingAction)
        {
            Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            blockingAction();
            stopWatch.Stop();
            return stopWatch.Elapsed;
        }

        static (TimeSpan, T) TimeIt<T>(Func<T> blockingAction)
        {
            Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            var result = blockingAction();
            stopWatch.Stop();
            return (stopWatch.Elapsed, result);
        }
    }
    public class NoDayFound : Exception { }
}
