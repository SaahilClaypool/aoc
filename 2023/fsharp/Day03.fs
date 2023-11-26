module Aoc.Solutions.Y2023.Day3

open System.Collections.Generic

open Aoc.Runner

type Day03_22() =
    inherit Day()

    let parse (line : string) =
        let midpoint = line.Length / 2
        (line[0..midpoint - 1], line[midpoint..])

    let priority (letter : char) =
        match letter with
        | x when  x >= 'a' && x <= 'z'  -> 1 + (int x) - (int 'a')
        | x when x >= 'A' && x <= 'Z' -> 1 + (int x) - (int 'A') + 26
        | _ -> raise (System.Exception "invalid letter")

    let overlap (ruck : string * string) =
        let (left, right) = ruck
        let overlap =
            (Set.ofSeq left)
            |> Set.intersect (Set.ofSeq right)
            |> Seq.head
        
        overlap

    let groupOverlap (group : seq<string>) =
        let sets = (group |> Seq.map Set.ofSeq)
        let first = Seq.head sets
        let rest = Seq.skip 1 sets 
        rest
        |> Seq.fold Set.intersect first
        |> Seq.head


    override _.SolveA input =
        input.Split('\n')
        |> Seq.map (parse >> overlap >> priority)
        |> Seq.sum
        |> string

    override _.SolveB input =
        input.Split '\n'
        |> Seq.chunkBySize 3
        |> Seq.map (groupOverlap >> priority)
        |> Seq.sum
        |> string

    override this.Tests =
        [
            Test("priority", "b", "2", (fun x -> x[0] |> priority |> string ))
            Test("priority", "L", "38", (fun x -> x[0] |> priority |> string ))
            Test(
                "overlap", "vJrwpWtwJgWrhcsFMMfFFhFp", "p",
                (parse >> trace >> overlap >> string)
            )
            Test(
                "A",
                """vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw""",
                "157",
                fun x -> this.SolveA x
            )
        ]
        |> List<Test>