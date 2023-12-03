module Aoc.Solutions.Y2023.Day03

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic



type Pos = { Row: int; Col: int }
type Number = { Pos: Pos; Num: int }
type Sym = { Pos: Pos; Symbol: string }

let adj pos =
    seq {
        for r in -1..1 do
            for c in -1..1 do
                yield { Row = pos.Row + r; Col = pos.Col + c }
    }

let pointsInString pos (str: string) = 
    seq {
        for i in 0..(str.Length - 1) do
            yield { pos with Col = pos.Col + i }
    }

let adjRange pos (text: string) =
    let pis = pointsInString pos text
    pis
    |> Seq.map adj
    |> Seq.concat
    |> Seq.filter (fun x -> not (Seq.contains x pis))
    |> Seq.distinct

let parse input =
    let numRe = Regex @"\d+"
    let symRe = Regex @"[^\d\.]+"
    let matchPositions (re: Regex) =
        lines input
        |> Seq.indexed
        |> Seq.map (fun (row, line) ->
            let matches = re.Matches line
            seq {
                for m in matches do
                    if not (String.IsNullOrWhiteSpace m.Value) then
                        yield (m.Value, { Col = m.Index; Row = row })
            }
        )
        |> Seq.concat
    let nums = matchPositions numRe
    let syms = matchPositions symRe
    
    (nums, syms)

type Day03() =
    inherit Day()

    let samp = 
        raw"""
            467..114..
            ...*......
            ..35..633.
            ......#...
            617*......
            .....+.58.
            ..592.....
            ......755.
            ...$.*....
            .664.598..
            """

    // sum of nums where syms next to exactly two nums
    override _.SolveA input =
        let (nums, syms) = input |> parse
        let numsMap =
            nums
            |> Seq.map (fun n ->
                pointsInString (snd n) (fst n)
                |> Seq.map (fun p -> (p, n)))
            |> Seq.concat
            |> Map.ofSeq
        
        let validSyms =
            syms
            |> Seq.choose  (fun s ->
                let a = adj (snd s)
                let adjNums = a |> Seq.choose numsMap.TryFind |> Seq.distinct
                match Seq.length adjNums with
                | x when x >= 1 -> Some (adjNums |> Seq.map (fst >> int))
                | _ -> None
            )
        
        validSyms
        |> Seq.map Seq.sum
        |> Seq.sum
        |> string

    override _.SolveB input =
        let (nums, syms) = input |> parse
        let numsMap =
            nums
            |> Seq.map (fun n ->
                pointsInString (snd n) (fst n)
                |> Seq.map (fun p -> (p, n)))
            |> Seq.concat
            |> Map.ofSeq
        
        let validSyms =
            syms
            |> Seq.choose  (fun s ->
                let a = adj (snd s)
                let adjNums = a |> Seq.choose numsMap.TryFind |> Seq.distinct
                match Seq.length adjNums with
                | x when x = 2 -> Some (adjNums |> Seq.map (fst >> int))
                | _ -> None
            )
        
        validSyms
        |> Seq.map Seq.product
        |> Seq.sum
        |> string


    override this.Tests =
        [
            Test(
                "a",
                samp,
                "4361",
                fun input -> this.SolveA input
            );
            Test(
                "b",
                samp,
                "467835",
                fun input -> this.SolveB input
            );
        ]
        |> List<Test>