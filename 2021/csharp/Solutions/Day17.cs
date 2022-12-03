using System.Text.RegularExpressions;

namespace Aoc.Solutions.Day17;

public class Day17 : Day
{
    static TargetArea Parse(string input) =>
        new Regex(@"target area: x=(?<MinX>.\d+)..(?<MaxX>.\d+), y=(?<MinY>.\d+)..(?<MaxY>.\d+)")
            .Match(input)
            .Then(l => { Log($"parse {l.Groups.Keys.Join("  ")}"); return l; })
            .Then(m =>
                new TargetArea(
                    new Pos(int.Parse(m.Groups["MinX"].Value), int.Parse(m.Groups["MinY"].Value)),
                    new Pos(int.Parse(m.Groups["MaxX"].Value), int.Parse(m.Groups["MaxY"].Value))
                )
            );
    public override string SolveA(string input)
    {
        var ta = Parse(input);
        var gMaxY = 0;
        foreach (var x in Range(0, 100))
        {
            foreach (var y in Range(0, 100))
            {
                Log($"x = {x} y = {y}");
                if (Simulate(new(x, y), ta, out var maxY))
                {
                    if (maxY > gMaxY)
                        gMaxY = maxY;
                }
            }
        }
        return gMaxY.ToString();
    }

    static bool Simulate(Vel v, TargetArea area, out int maxY)
    {
        var probe = new Probe(new(0, 0), v);
        maxY = 0;
        while (!Bad(probe, area))
        {
            if (probe.P.Y > maxY)
                maxY = probe.P.Y;

            if (Hit(probe, area))
                return true;

            probe = probe.Step();
        }
        return false;
    }

    static bool Bad(Probe p, TargetArea area)
    {
        var badX = p.P.X < area.Min.X && p.V.X <= 0;

        var badY = p.P.Y < area.Min.Y && p.V.Y <= 0;

        return badX || badY;
    }

    static bool Hit(Probe p, TargetArea area) =>
        p.P.X >= area.Min.X && p.P.X <= area.Max.X && p.P.Y >= area.Min.Y && p.P.Y <= area.Max.Y;

    public override string SolveB(string input)
    {
        var ta = Parse(input);
        var validV = 0;
        foreach (var x in Range(0, ta.Max.X * 2))
        {
            foreach (var y in Range(ta.Min.Y, 1000))
            {
                Log($"x = {x} y = {y}");
                if (Simulate(new(x, y), ta, out var _))
                {
                    validV++;
                }
            }
        }
        return validV.ToString();
    }

    public Day17()
    {
        Tests = new()
        {
            new("A", @"target area: x=20..30, y=-10..-5", "45", SolveA),
            new("B", @"target area: x=20..30, y=-10..-5", "112", SolveB)
        };
    }
}

record Probe(Pos P, Vel V)
{
    public Probe Step() =>
        this with
        {
            P = new(P.X + V.X, P.Y + V.Y),
            V = new(
                V.X > 0 ?
                    V.X - 1 :
                    V.X < 0 ?
                        V.X + 1 :
                        0,
                V.Y - 1
            )
        };
}
record Pos(int X, int Y);
record Vel(int X, int Y);

record TargetArea(Pos Min, Pos Max);