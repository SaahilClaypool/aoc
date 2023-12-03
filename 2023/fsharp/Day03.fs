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


let parse (input: string): (Map<Pos, Thing>) =
    let lines = input.Split '\n'

    let things =
        seq {
            for (row, line) in Seq.indexed lines do
                let mutable inNum = false
                let mutable currentNum = ""
                let mutable numStart = 0
                for (col, c) in Seq.indexed line do
                    if (Char.IsDigit c) && (not inNum) then
                        inNum <- true
                        numStart <- col
                        currentNum <- currentNum + string c
                        None |> ignore
                    elif (Char.IsDigit c) && (inNum) then
                        currentNum <- currentNum + string c
                        None |> ignore
                    elif inNum then // end num  
                        inNum <- false
                        let num = currentNum
                        currentNum <- ""
                        yield Num({Pos= {Row= row; Col= col - num.Length}; Num = int num })
                    if (c = '.') then
                        None |> ignore
                    elif (not (Char.IsDigit c)) then
                        inNum <- false
                        yield Sym({Pos = {Row= row; Col= col }; Symbol = string c })
                if inNum then
                    yield Num({Pos= {Row= row; Col= line.Length - currentNum.Length }; Num = int currentNum })
        }
        |> List.ofSeq

    things
    |> Seq.map (fun thing ->
        match thing with
        | Num num -> (num.Pos, Num(num))
        | Sym sym -> (sym.Pos, Sym(sym))
    )
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

    override _.SolveA input =
        let things = input |> parse
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
        dump (numsNextToSym |> Seq.map (fun x -> x.Num))
        
        numsNextToSym
        |> Seq.map (fun x -> x.Num)
        |> Seq.sum
        |> string

    override _.SolveB input =
        ""
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
                fun input -> parse input |> Seq.length |> string
            );
            Test(
                "parse",
                raw"""
                467..114..
                ...*......
                ..35..633.
                """,
                "12",
                fun input -> parse input |> mapToExpanded |> Seq.length |> string
            );
            Test(
                "a",
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
                    """,
                "4361",
                fun input -> this.SolveA input
            );
        ]
        |> List<Test>