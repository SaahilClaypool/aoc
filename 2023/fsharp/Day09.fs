module Aoc.Solutions.Y2023.Day09

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic


let parse input =
    lines input
    |> Seq.map (fun x -> x.Split ' ' |> Seq.map int64 |> Seq.toList)
    |> Seq.toList


let rec nextValue input =
    if input |> Seq.all (fun x -> x = 0L)  then
        0L
    else
        let differences =
            input
            |> Seq.pairwise 
            |> Seq.map (fun (left, right) -> right - left)
            |> List.ofSeq

        let nextRowValue = nextValue differences
        (Seq.last input) + nextRowValue


let sample = raw"""
    0 3 6 9 12 15
    1 3 6 10 15 21
    10 13 16 21 30 45
    """

type Day09() =
    inherit Day()

    override _.SolveA input =
        let lines = parse input
        let nextValues = lines |> Seq.map nextValue
        
        Seq.sum nextValues |> string

    override _.SolveB input =
        let lines = parse input
        let nextValues = lines |> Seq.map (Seq.rev >> Seq.toList) |> Seq.map nextValue

        Seq.sum nextValues |> string
    override this.Tests =
        [
            Test("a", sample, "114", fun x -> this.SolveA x);
            Test("b", sample, "2", fun x -> this.SolveB x);
        ]
        |> List<Test>