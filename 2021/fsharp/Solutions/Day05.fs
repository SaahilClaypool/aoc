module Aoc.Solutions.Day05

open Aoc.Runner

type Point = int * int
type Line = { A: Point; B: Point }

let parseLine (line: string) =
    let parts = line.Split(' ')

    let parsePoint (point: string) =
        let nums = point.Split(',')
        (int nums[0], int nums[1])

    let first = parsePoint parts[0]
    let last = parsePoint parts[^0]

    { A = first; B = last}

let parse (input: string) =
    input.Split('\n')
    |> Seq.map parseLine

let pointsOnLine (line: Line) diag =
    let (x1, y1) = line.A
    let (x2, y2) = line.B
    let (xs, ys) = if x1 < x2 then (x1, y1) else (x2, y2)
    let (xe, ye) = if x1 < x2 then (x2, y2) else (x1, y1)
    if x2 - x1 = 0 then
        let slope = if y2 > y1 then 1 else -1
        seq {
            for i in 0 .. abs (y2 - y1) ->
                (x1, y1 + i * slope)
        } |> Seq.toList
    else if y2 - y1 = 0 then
        let slope = if x2 > x1 then 1 else -1
        seq {
            for i in 0 .. abs (x2 - x1) ->
                (x1 + i * slope, y1)
        } |> Seq.toList
    else if diag then
        let yslope = if ye > ys then 1 else -1
        let xslope = if xe > xs then 1 else -1
        seq {
            for i in 0 .. abs (ye - ys) ->
                (xs + i * xslope, ys + i * yslope)
        } |> Seq.toList
    else
        Seq.empty |> Seq.toList
    
    
type Day05() =
    inherit Day()

    override _.SolveA input =
        let parsed = parse input
        let points =
            parsed
            |> Seq.map (fun line -> pointsOnLine line false)
        let score =
            points
            |> Seq.concat
            |> Seq.countBy (fun v -> v)
            |> Seq.filter (fun g ->
                let (_, cnt) = g
                cnt >= 2)
            |> Seq.length
        score |> string

    override _.SolveB input =
        let parsed = parse input
        let points =
            parsed
            |> Seq.map (fun line -> pointsOnLine line true)
        let score =
            points
            |> Seq.concat
            |> Seq.countBy (fun v -> v)
            |> Seq.filter (fun g ->
                let (_, cnt) = g
                cnt >= 2)
            |> Seq.length
        score |> string