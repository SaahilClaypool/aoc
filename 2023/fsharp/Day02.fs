module Aoc.Solutions.Y2023.Day02

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic



type Game = (int * seq<seq<int * string>>)

// Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
let parseLine (line): Game =
    let setRegex = Regex "Game (\\d+): (.*)"
    let lineMatch = setRegex.Match line
    let parseGroup (group: string) =
        match group.Split ' ' with
        | [|count; color|] -> ((int count), color)
        | _ -> raise (Exception "bad input")
    let parseSet (set: string) =
        set.Split ", "
        |> Seq.map parseGroup

    let setNumber = lineMatch.Groups[1].Value |> int
    let sets =
        lineMatch.Groups[2].Value.Split("; ")
        |> Seq.map parseSet
        |> Array.ofSeq

    (setNumber, sets) 

let isValidSet (maxCounts: Map<string, int>) (set: seq<int * string>) =
    let invalid =
        set
        |> Seq.filter (fun (count, color) ->
            count > maxCounts[color])
            
    if (Seq.length invalid > 0) then dump invalid
    (Seq.length invalid) = 0


let isValidGame maxCounts (game: Game) =
    let sets = (snd game) 
    let validSets = Seq.filter (isValidSet maxCounts) sets
    (Seq.length validSets) = (Seq.length sets)
    
let setPower (game: Game) =
    let sets = (snd game)
    sets
    |> Seq.concat
    |> Seq.groupBy snd
    |> Seq.map (fun group ->
        (snd group)
        |> Seq.map fst
        |> Seq.max)
    |> Seq.fold
            (fun state seed -> state * seed)
        1


let sampleA =
    """Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"""

type Day02() =
    inherit Day()

    override _.SolveA input =
        // only 12 red cubes, 13 green cubes, and 14 blue cubes
        let maxForColors =
            [("red", 12); ("green", 13); ("blue", 14)] 
            |> Map.ofSeq
        let lines =
            input.Split '\n'
            |> Seq.map parseLine

        let validGames: seq<Game> =
            lines 
            |> Seq.filter (isValidGame maxForColors)
        validGames
        |> Seq.map fst
        |> Seq.sum
        |> string

    override _.SolveB input =
        let games =
            input.Split '\n'
            |> Seq.map parseLine
        
        games
        |> Seq.map setPower
        |> Seq.sum
        |> string
    
    override this.Tests =
        [
            Test("parse", "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue", "2",
                fun x ->
                    let (id, sets) = parseLine x
                    $"{id}"
            );
            Test("a",
                sampleA, "8",
                fun x -> this.SolveA x
            );
            Test("b",
                sampleA, "2286",
                fun x -> this.SolveB x
            );
        ]
        |> List<Test>