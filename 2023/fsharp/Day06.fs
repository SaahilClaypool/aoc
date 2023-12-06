module Aoc.Solutions.Y2023.Day06

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic

let numbers (line: string) =
    let matches = Regex.Matches(line, @"\d+")
    matches
    |> Seq.map ((fun x -> x.Value) >> int64)

let parse (input: string) =
    match lines input with
    | [|first; second|] ->
        let raceNums = numbers first
        let maxNums = numbers second
        Seq.zip raceNums maxNums
    | _ -> raise (Exception "bad input")

let parseB (input: string) =
    match lines input with
    | [|first; second|] ->
        let raceNums = numbers first
        let maxNums = numbers second
        (int64(String.Join("", raceNums)), int64(String.Join("", maxNums)))
    | _ -> raise (Exception "bad input")

let dist h t =
    (int64 (t - h)) * h

let distByHoldTime (raceTime: int64) =
    seq {
        let mutable i = 0L
        while i < raceTime do
            yield dist i raceTime
            i <- i + 1L
    }
    |> List.ofSeq

let numBetterRaces (raceTime, maxValue) =
    distByHoldTime raceTime
    |> Seq.filter (fun x -> x > int64(maxValue))
    |> Seq.length
    |> int64

type Day06() =
    inherit Day()

    override _.SolveA input =
        let races = parse input
        
        let waysToWin =
            races
            |> Seq.map numBetterRaces
        
        waysToWin
        |> Seq.productL
        |> string

    override _.SolveB input =
        let race = parseB input
        
        let waysToWin = numBetterRaces race
        
        waysToWin
        |> string
    

    member _.sample = raw"""
    Time:      7  15   30
    Distance:  9  40  200
    """

    override this.Tests =
        [
            Test("a", this.sample, "288", fun x -> this.SolveA x);
            Test("b", this.sample, "71503", fun x -> this.SolveB x);
        ]
        |> List<Test>