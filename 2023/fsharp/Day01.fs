module Aoc.Solutions.Y2023.Day1

open Aoc.Runner
open System.Collections.Generic

let parseLine line =
    line |> Seq.filter System.Char.IsDigit

let parseInput (str: string) =
    str.Split '\n'



let parseB (line: string) =
    seq {
        let mutable i = 0
        while i < line.Length do
            let temp = line[i..]
            let digit =
                if System.Char.IsDigit temp[0] then
                    let x = temp[0]
                    x
                elif temp.StartsWith "zero" then
                    '0'
                elif temp.StartsWith "one" then
                    '1'
                elif temp.StartsWith "two" then
                    '2'
                elif temp.StartsWith "three" then
                    '3'
                elif temp.StartsWith "four" then
                    '4'
                elif temp.StartsWith "five" then
                    '5'
                elif temp.StartsWith "six" then
                    '6'
                elif temp.StartsWith "seven" then
                    '7'
                elif temp.StartsWith "eight" then
                    '8'
                elif temp.StartsWith "nine" then
                    '9'
                else
                    'x'
            i <- i + 1
            if System.Char.IsDigit digit then
                yield digit
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

    override _.SolveA input =
        input
        |> parseInput
        |> Seq.map (fun line  ->
            let parsed = parseLine line
            let first = Seq.head parsed
            let last = Seq.last parsed
            $"{first}{last}"
        )
        |> Seq.map int
        |> Seq.sum
        |> string

    override _.SolveB input =
        input
        |> parseInput
        |> Seq.map (fun line  ->
            let parsed = parseB line
            let first = Seq.head parsed
            let last = Seq.last parsed
            $"{first}{last}"
        )
        |> Seq.map int
        |> Seq.sum
        |> string
    
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