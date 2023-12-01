module Aoc.Solutions.Y2023.Day1

open Aoc.Runner
open System.Collections.Generic

let parseA line =
    line |> Seq.filter System.Char.IsDigit

let parseInput (str: string) =
    str.Split '\n'

let parseB (line: string) =
    let digits = [| "zero"; "one";"two";"three";"four";"five";"six";"seven";"eight";"nine" |] |> Seq.indexed
    let nums = [| 0;1;2;3;4;5;6;7;8;9; |] |> Seq.map string |> Seq.indexed
    let map = Seq.concat [|digits; nums |]
    seq {
        for i in 0..line.Length do
            let m = Seq.tryFind (fun ((v: int), (k: string)) -> line[i..].StartsWith k) map
            match m with
            | Some value ->
                yield (fst value).ToString()[0]
            | None -> ()
    }
    
let sampleA = """1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet"""

let sampleB = """two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen"""

type Day01() =
    inherit Day()

    let solve parser input =
        input
        |> parseInput
        |> Seq.map (fun line  ->
            let parsed = parser line
            let first = Seq.head parsed
            let last = Seq.last parsed
            $"{first}{last}"
        )
        |> Seq.map int
        |> Seq.sum
        |> string


    override _.SolveA input = input |> solve parseA

    override _.SolveB input = input |> solve parseB
    
    override this.Tests =
        [
            Test(
            "a",
            sampleA,
            "142", fun x -> this.SolveA x);
            Test(
            "b",
            sampleB,
            "281", fun x -> this.SolveB x)
        ]
        |> List<Test>