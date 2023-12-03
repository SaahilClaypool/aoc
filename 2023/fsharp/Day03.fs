module Aoc.Solutions.Y2023.Day03

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic



type Pos = { Row: int; Col: int }
type Number = { Pos: Pos; Num: int }
type Sym = { Pos: Pos; Symbol: string }

type Thing =
| Num of Number
| Sym of Sym

let adj pos =
    seq {
        for r in -1..1 do
            for c in -1..1 do
                yield { Row = pos.Row + r; Col = pos.Col + c }
    }


let parseRegex (input: string) =
    // match 
    let re = Regex("(\\d+)|([^\\d\\.]+)")
    seq {
        for (row, line) in (input.Split '\n') |> Seq.indexed do
            let matches = re.Matches(line)
            for m in matches do
                let numMatch = m.Groups[1]
                let symMatch = m.Groups[2]
                if numMatch.Index >= 0 && numMatch.Value.Length > 0 then
                    let p = { Row = row; Col = numMatch.Index }
                    yield (p, Num({ Pos = p; Num = int numMatch.Value }))
                elif symMatch.Index >= 0 && symMatch.Value.Length > 0 then
                    let p = { Row = row; Col = symMatch.Index }
                    yield (p, Sym({ Pos = p; Symbol = symMatch.Value }))
    }
    |> Map.ofSeq

let mapToExpanded (inp: Map<Pos, Thing>) =
    seq {
        for kvp in inp do
            let v = kvp.Value
            let pos = kvp.Key
            match v with
            | Num num ->
                for i in 0..((string num.Num).Length - 1) do
                    yield ({pos with Col = pos.Col + i }, v)
            | _ -> yield (pos, v)
    }
    |> Map.ofSeq

let adjPointsToNum (num: Number) =
    let pointsInNum = 
        seq {
            for i in 0..((string num.Num).Length - 1) do
                yield { num.Pos with Col = num.Pos.Col + i }
        }
    pointsInNum
    |> Seq.map adj
    |> Seq.concat
    |> Seq.filter (fun x -> not (Seq.contains x pointsInNum ))
    |> Seq.distinct


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

    override _.SolveA input =
        let things = input |> parseRegex
        let syms =
            things.Values
            |> Seq.choose 
                (fun x ->
                    match x with
                    | Sym sym -> Some(sym)
                    | _ -> None)
        
        let symPos = syms |> Seq.map (fun x -> x.Pos)

        let nums =
            things.Values
            |> Seq.choose 
                (fun x ->
                    match x with
                    | Num num -> Some(num)
                    | _ -> None)

        let numsNextToSym =
            nums
            |> Seq.filter (fun num ->
                let adj = adjPointsToNum num
                let adjacentSymbol =
                    Seq.tryFind (fun pos ->
                        Seq.contains pos symPos
                    ) adj
                adjacentSymbol.IsSome
            )
        
        numsNextToSym
        |> Seq.map (fun x -> x.Num)
        |> Seq.sum
        |> string

    override _.SolveB input =
        let parsed = (parseRegex input) |> mapToExpanded
        let possibleGears =
            parsed.Values
            |> Seq.choose (fun x ->
                match x with
                | Sym sym when sym.Symbol = "*" -> Some(sym)
                | _ -> None
            )
        let numbers = 
            parsed
            |> Seq.map (fun kvp -> (kvp.Key, kvp.Value))
            |> Seq.choose (fun x ->
                match x with
                | (x, Num num) -> Some((x, num))
                | _ -> None
            )
        
        let validGears = 
            possibleGears
            |> Seq.choose (fun gear ->
                let adjPositionsToGear = adj gear.Pos
                let adjacentNumbers =
                    numbers
                    |> Seq.filter (fun num ->
                        Seq.contains (fst num) adjPositionsToGear
                    )
                    |> Seq.distinctBy (fun x -> (snd x).Pos)
                if Seq.length adjacentNumbers = 2 then
                    Some(adjacentNumbers)
                else
                    None
            )
        
        validGears
        |> Seq.map
            (fun gearNumbers ->
                gearNumbers
                |> Seq.map (fun n -> (snd n).Num)
                |> Seq.fold (fun state x -> state * x) 1)
            
        |> Seq.sum
        |> string

    override this.Tests =
        [
            Test(
                "parse",
                raw"""
                467..114..
                ...*......
                ..35..633.
                """,
                "5",
                fun input -> parseRegex input |> Seq.length |> string
            );
            Test(
                "parse",
                raw"""
                467..114..
                ...*......
                ..35..633.
                """,
                "12",
                fun input -> parseRegex input |> mapToExpanded |> Seq.length |> string
            );
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