module Aoc.Solutions.Day09

open Aoc.Runner
open System.Collections.Generic

let surrounding (r, c) (heights: int[][]) =
    let rows = Seq.length heights
    let cols = Seq.length heights[0]
    seq {
        for ri in r - 1 .. r + 1 do
            for ci in c - 1 .. c + 1 ->
                (ri, ci)
    }
    |> Seq.filter (fun (ri, ci) ->
                ri >= 0 && ri < rows && ci >= 0 && ci < cols && (ci = c || ri = r) && not(ri = r && ci = c))

let surroundingValue point heights =
    surrounding point heights
    |> Seq.map (fun (r, c) -> heights[r][c])

let isLow (r, c) heights =
    surrounding (r, c) heights
    |> Seq.forall (fun (ri, ci) ->
        heights[r][c] < heights[ri][ci])

let risk (r, c) heights =
    if isLow (r, c) heights then
        heights[r][c] + 1
    else
        0

let points (heights: int[][]) =
    seq {
        for r in 0 .. Seq.length heights - 1 do
            for c in 0 .. Seq.length heights[0] - 1 ->
                (r, c)
    }

let basin point (heights: int[][]) =
    let basin = new HashSet<int * int>()
    let rows = Seq.length heights
    let cols = Seq.length heights[0]
    let r, c = point
    basin.Add(point) |> ignore

    let inBasin somePoint =
        surrounding somePoint heights
        |> Seq.exists (fun another -> basin.Contains(another))

    let mutable ring = 1
    let mutable matches = true

    while matches = true do
        matches <- false
        for ri in r - ring .. r + ring + 1 do
            for ci in c - ring .. c + ring + 1 do
                if ci >= 0 && ci < cols && ri >= 0 && ri < rows && heights[ri][ci] <> 9 then
                    let anySurrounding = inBasin (ri, ci)
                    if anySurrounding && not(basin.Contains((ri, ci))) then
                        basin.Add((ri, ci)) |> ignore
                        matches <- true
        ring <- ring + 1

    basin

let inline charToInt c = int c - int '0'
let parse (input: string) =
    input.Split('\n')
    |> Array.map (fun line -> line.Trim() |> Seq.toArray |> Array.map charToInt)

type Day09() =
    inherit Day()

    override _.SolveA input =
        let heights = parse input
        heights
        |> points
        |> Seq.map (fun point -> risk point heights)
        |> Seq.sum
        |> string

    override _.SolveB input =
        let heights = parse input
        let lows = heights |> points |> Seq.filter (fun p -> isLow p heights)
        lows
        |> Seq.map (fun p -> basin p heights)
        |> Seq.map Seq.length
        |> Seq.sortDescending
        |> Seq.take 3
        |> Seq.fold (fun xs x -> x * xs) 1
        |> string