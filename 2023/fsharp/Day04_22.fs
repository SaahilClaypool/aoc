module Aoc.Solutions.Y2023.Day04_22

open Aoc.Runner
open System.Collections.Generic

type assignment = int * int


let parseAssignment (a: string) =
    match (a.Split '-' |> Seq.map int |> Array.ofSeq) with
    | [|left; right|] -> (left, right)
    | _ -> raise (new System.Exception "invalid assignment..")
    
let parse (line: string) =
    match (line.Split ',') with
    | [|left; right|] -> (parseAssignment left, parseAssignment right)
    | _ -> raise (new System.Exception "invalid line..")

let parseInput (str: string) =
    str.Split '\n'
    |> Seq.map parse

let fullOverlap (a: assignment) (b: assignment) =
    let l1, r1 = a
    let l2, r2 = b
    l1 <= l2 && r1 >= r2 || l2 <= l1 && r2 >= r1

let sampleA  = """2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8"""

type Day04_22() =
    inherit Day()

    override _.SolveA input =
        input |> (parseInput >> Seq.filter (fun (a, b) -> fullOverlap a b) >> Seq.length >> string)

    override _.SolveB input =
        ""
    
    override this.Tests =
        [
            Test(
            "a",
            sampleA,
            "(2, 4)", parseInput >> Seq.head >> fst >>string);
            Test(
            "b",
            sampleA,
            "2", fun x -> this.SolveA x)
        ]
        |> List<Test>