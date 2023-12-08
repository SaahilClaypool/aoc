module Aoc.Solutions.Y2023.Day08

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic

let parse input =
    let lines = lines input
    (lines[0],
        lines[2..]
        |> Seq.map (fun l ->
            match l.Split '=' with
            | [|left; right|] ->
                (left.Trim(), Regex.Matches(right, @"\w+") |> Seq.map (fun x -> x.Value) |> Seq.toList)
            | _ -> raise (Exception $"bad line {l}")
        )
        |> Map.ofSeq
    )


type Day08() =
    inherit Day()

    override _.SolveA input =
        let (dirs, nodes) = parse input
        let start = "AAA"
        let endz = "ZZZ"
        let mutable cur = start
        let mutable i = 0 
        while endz <> cur do
            let d = dirs[i % dirs.Length]
            i <- i + 1
            cur <-
                match d with
                | 'L' -> nodes[cur][0]
                | 'R' -> nodes[cur][1]
                | _ -> raise (Exception "impossible")
        i |> string

    // idea 2: for each path, find all the values it hits ends on, and it's cycle length
    // then line up the cycles
    // say you have 3 5 7, length 9
    // and you have 2 3 8 length 11
    // you could generate all the numbers it ends at (skipping the rest)
    // off to work
    override _.SolveB input =
        let (dirs, nodes) = parse input
        let mutable currents =
            nodes
            |> Seq.choose (fun x ->
                match x.Key.EndsWith "A" with
                | true -> Some x.Key
                | false -> None
            )
            |> List.ofSeq
        let allEnds () =
            currents
            |> Seq.filter (fun x -> not (x.EndsWith "Z"))
            |> Seq.length = 0
        let mutable i = 0 
        while not (allEnds ()) do
            let d = dirs[i % dirs.Length]
            i <- i + 1
            currents <- seq {
                for path in currents do
                    yield
                        match d with
                        | 'L' -> nodes[path][0]
                        | 'R' -> nodes[path][1]
                        | _ -> raise (Exception "impossible")

            } |> List.ofSeq
        i |> string

    member _.Sample = raw"""
    RL

    AAA = (BBB, CCC)
    BBB = (DDD, EEE)
    CCC = (ZZZ, GGG)
    DDD = (DDD, DDD)
    EEE = (EEE, EEE)
    GGG = (GGG, GGG)
    ZZZ = (ZZZ, ZZZ)
    """

    member _.Sample2 = raw"""
    LLR

    AAA = (BBB, BBB)
    BBB = (AAA, ZZZ)
    ZZZ = (ZZZ, ZZZ)
    """

    member _.Sample3 = raw"""
    LR

    11A = (11B, XXX)
    11B = (XXX, 11Z)
    11Z = (11B, XXX)
    22A = (22B, XXX)
    22B = (22C, 22C)
    22C = (22Z, 22Z)
    22Z = (22B, 22B)
    XXX = (XXX, XXX)
    """
    override this.Tests =
        [
            Test("a", this.Sample, "2", fun x -> this.SolveA x);
            Test("a2", this.Sample2, "6", fun x -> this.SolveA x);
            Test("b1", this.Sample3, "6", fun x -> this.SolveB x);
        ]
        |> List<Test>