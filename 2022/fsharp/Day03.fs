module Aoc.Solutions.Day03

open Aoc.Runner

let score (c: char) = 
    1 + 
    (if System.Char.IsUpper(c) then
        (int)c - (int)'A' + 26
    else (int)c - (int)'a')


type Day03() =
    inherit Day()

    override _.SolveA input =
        input.Split '\n'
        |> Seq.map (fun line ->
            let half = line.Length / 2
            let left = line[..(half - 1)]
            let right = line[half..]
            (Set.intersect (Set.ofSeq left) (Set.ofSeq right)) |> Seq.head)
        |> Seq.map score
        |> Seq.sum
        |> string


    override _.SolveB input =
        input.Split '\n'
        |> Seq.map Set.ofSeq
        |> Seq.chunkBySize 3
        |> Seq.map Set.intersectMany
        |> Seq.map Seq.head
        |> Seq.map score
        |> Seq.sum
        |> string