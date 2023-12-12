module Aoc.Solutions.Y2023.Day11

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic

let parse (input: string) =
    input
    |> lines
    |> Seq.indexed
    |> Seq.collect (fun (row, line) ->
        line
        |> Seq.indexed
        |> Seq.choose (fun (col, c) ->
        match c with 
        | '#' -> Some (int64 row, int64 col)
        | _ -> None
        )
    )

let distance (a: (int64 * int64)) (b: (int64 * int64)) =
    Math.Abs((fst a) - (fst b)) + Math.Abs((snd a) - (snd b))


let expandGalaxy (factor: int64) (galaxyRows: Set<int64>) (galaxyCols: Set<int64>) (galaxy: (int64 * int64)) =
    let (row, col) = galaxy
    let occupiedRows = galaxyRows |> Seq.filter (fun r -> r < row) |> Seq.length |> int64
    let occupiedCols = galaxyCols |> Seq.filter (fun c -> c < col) |> Seq.length |> int64
    let emptyRows = row - occupiedRows
    let emptyCols = col - occupiedCols
    let expanded = emptyRows * factor + occupiedRows, emptyCols * factor + occupiedCols

    expanded

let allPairCombos a =
    seq {
        for i in 0..(Seq.length a - 1) do
            for j in (i + 1)..(Seq.length a - 1) do
                a |> Seq.item i, a |> Seq.item j
    }

let solve factor input =
    let galaxies = parse input
    let occupiedRows = galaxies |> Seq.map fst |> Set.ofSeq
    let occupiedCols = galaxies |> Seq.map snd |> Set.ofSeq
    let expanded = galaxies |> Seq.map (expandGalaxy factor occupiedRows occupiedCols) |> Seq.toList
    let pairs = expanded |> allPairCombos |> List.ofSeq
    let totalDistance = 
        pairs
        |> Seq.map (fun (left, right) ->
            let d = distance left right
            d)
        |> Seq.sum

    totalDistance |> string

let sample = raw"""
...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#.....
"""

type Day11() =
    inherit Day()

    override _.SolveA input = input |> solve 2L

    override _.SolveB input = input |> solve 1000000L
        
    override this.Tests =
        [
            Test("a", sample, "374", fun x -> this.SolveA x);
        ]
        |> List<Test>
    