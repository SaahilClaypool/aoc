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

let followPath (dirs: string) (nodes: Map<string, list<string>>) (start: string) =
        let mutable cur = start
        let mutable i = 0 
        let make_state i pos =
            $"{pos}_{i % dirs.Length}"
        let mutable states = List.empty<string>
        seq {
            let mutable state = ""
            state <- make_state i cur
            while not (List.contains state states) do
                states <- List.append states [state]
                let d = dirs[i % dirs.Length]
                i <- i + 1
                cur <-
                    match d with
                    | 'L' -> nodes[cur][0]
                    | 'R' -> nodes[cur][1]
                    | _ -> raise (Exception "impossible")
                yield (cur, (states, state))
                state <- make_state i cur
                // [{"Item1":"11B","Item2":{"Item1":["11A_0"],"Item2":"11A_0"}},{"Item1":"11Z","Item2":{"Item1":["11A_0","11B_1"],"Item2":"11B_1"}},{"Item1":"11B","Item2":{"Item1":["11A_0","11B_1","11Z_0"],"Item2":"11Z_0"}}]
        }

let findRecurrence dirs nodes start =
    let path =
        followPath dirs nodes start
        |> Seq.toList
    
    let (states, state) = Seq.last path |> snd
    let offset = (states |> Seq.findIndex (fun x -> x = state)) - 1
    dump (path |> ResizeArray)
    let cycleLength = (states |> Seq.rev |> Seq.skip 1 |> Seq.findIndex (fun x -> x = state))
    
    let v =
        (offset,
        cycleLength,
        path
        |> Seq.indexed
        |> Seq.filter (fun x ->
            (fst (snd x)).EndsWith("Z"))
        |> Seq.map fst
        |> Seq.map (fun i -> (i - offset + cycleLength) % cycleLength)
        |> Seq.toList)
    let (_, _, wins) = v
    
    printfn $"from {start} we found the path {path |> Seq.map fst |> json} winning at {v}"

    v

type Day08() =
    inherit Day()

    override _.SolveA input =
        let (dirs, nodes) = parse input
        let start = "AAA"
        let endz = "ZZZ"
        let path =
            followPath dirs nodes start
            |> Seq.takeWhile (fun cur ->
                (fst cur) <> endz)
            |> Seq.length
        (path + 1) |> string

    // idea 2: for each path, find all the values it hits ends on, and it's cycle length
    // then line up the cycles
    // say you have 3 5 7, length 9
    // and you have 2 3 8 length 11
    // you could generate all the numbers it ends at (skipping the rest)
    // off to work
    override _.SolveB input =
        let (dirs, nodes) = parse input
        let starts =
            nodes
            |> Seq.filter (fun x -> x.Key.EndsWith "A")
        
        let recs = starts |> Seq.map _.Key |> Seq.map (findRecurrence dirs nodes) |> List.ofSeq
        lg (recs |> json)

        let wouldEnd i ((offset, length, offsets): int * int * list<int>) =
            let cycleOffset = (i - offset + length) % length
            lg $"cycle offset { cycleOffset } ({offset} {length} {i})"
            let endings =
                offsets
                |> Seq.filter (fun nodeOffset -> nodeOffset = cycleOffset)
                |> Seq.length
            printfn $"would {(offset, length, offsets)} end at {i + 1}? {endings}"
            endings > 1
        
        let allEnd i = (recs |> Seq.filter (wouldEnd i) |> Seq.length) = recs.Length

        let mutable i = 0
        while not (allEnd i) && i <= 16 do
            printfn $"{i}\n\n"
            i <- i + 1
        
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
            // Test("a", this.Sample, "2", fun x -> this.SolveA x);
            // Test("a2", this.Sample2, "6", fun x -> this.SolveA x);
            Test("b1", this.Sample3, "6", fun x -> this.SolveB x);
        ]
        |> List<Test>