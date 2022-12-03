module Aoc.Solutions.Day01

open Aoc.Runner

// alias CList to c# list for interop
type CList<'T> = System.Collections.Generic.List<'T>

let parseInput (text: string) = text.Split('\n') |> Seq.map int

let pairs sequence =
    seq {
        for i in 1 .. (List.length sequence - 1) do
            (sequence.[i - 1], sequence.[i])
    }

let triples sequence =
    seq {
        for i in 2 .. (List.length sequence - 1) do
            (sequence.[i - 2], sequence.[i - 1], sequence.[i])
    }

let solveA (pairs: seq<(int * int)>) =
    pairs
    |> Seq.map (fun (prev, next) -> if next > prev then 1 else 0)
    |> Seq.sum

type Day01() =
    inherit Day()

    override _.SolveA(input) =
        let inp = parseInput input |> List.ofSeq
        let paired = pairs inp
        (solveA paired).ToString()

    override _.SolveB(input) =
        let inp = parseInput input |> List.ofSeq
        let trips = triples inp

        let avg =
            trips
            |> Seq.map (fun (a, b, c) -> a + b + c)
            |> List.ofSeq

        let paired = pairs avg
        (solveA paired).ToString()

    override this.Tests =
        [ new Test(
              name = "basic part A",
              input =
                  @"199
200
208
210
200
207
240
269
260
263",
              expectedOutput = "7",
              solve = fun x -> this.SolveA(x)
          ) ]
        |> CList
