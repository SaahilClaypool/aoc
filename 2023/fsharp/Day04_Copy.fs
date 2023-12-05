module Aoc.Solutions.Y2023.Day04_Copy

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic

// inpsired by https://github.com/encse/adventofcode/tree/master/2023/Day04

let parseCard (line: string) =
    let parts = line.Split(':', '|')
    let nums i =
        Regex.Matches(parts[i], @"\d+")
        |> Seq.map (fun x -> x.Value)
        |> Set.ofSeq
    let l = nums 1
    let r = nums 2
    (Set.intersect l r).Count

type Day04_Copy() =
    inherit Day()

    override _.SolveA input =
        lines input
        |> Seq.map parseCard
        |> Seq.choose (fun x ->
            match x with
            | x when x > 0 -> Some (Math.Pow(2, float(x - 1)))
            | _ -> None)
        |> Seq.sum
        |> string

    override _.SolveB input =
        let cards = input |> lines |> Seq.map parseCard |> ResizeArray
        let counts = cards |> Seq.map (fun _ -> 1) |> ResizeArray
        for i in 0..(cards.Count - 1) do
            let (matches, count) = (cards[i], counts[i])
            for j in 0..(matches - 1) do
                counts[i + j + 1] <- counts[i + j + 1] + count 
        counts |> Seq.sum |> string

    override this.Tests =
        [
            Test("a", "sample", "output", fun x -> this.SolveA x);
        ]
        |> List<Test>