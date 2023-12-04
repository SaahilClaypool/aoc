module Aoc.Solutions.Y2023.Day04

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic

let parseLine (line: string) =
    let nums = (line.Split ": ")[1]
    match nums.Split(" | ") with
    | [|left; right|] -> (
        (left.Split(' ', StringSplitOptions.RemoveEmptyEntries)) |> Seq.map int |> Set.ofSeq,
        (right.Split(' ', StringSplitOptions.RemoveEmptyEntries)) |> Seq.map int |> Set.ofSeq)
    | _ -> raise (Exception "bad input")

let parse input =
    lines input
    |> Seq.map parseLine

let matches ((left: Set<int>), (right: Set<int>)) =
    Set.intersect left right

let score (left, right) =
    let m = matches (left, right)
    let m = Seq.length m - 1
    Math.Pow(2, m) |> int



let rec treeScoreCard (state: Dictionary<int, ResizeArray<int>>) (cards: list<(Set<int> * Set<int>)>) (cardNum: int) =
    match state.GetValueOrDefault cardNum with
    | null ->
        let matches = matches cards[cardNum - 1] |> Seq.length
        let allDerivativeNumbers =
            match matches with
            | 0 -> Seq.empty<int>
            | x -> 
                let copyCards = seq { (cardNum + 1)..(cardNum + matches) }
                let copyOutputs =
                    copyCards
                    |> Seq.map (fun x -> treeScoreCard state cards x)
                copyOutputs |> Seq.concat
        state.Add(cardNum, Seq.concat [ seq  [cardNum]; allDerivativeNumbers ] |> ResizeArray)
        state[cardNum]
    | x -> x


let treeScore (cards: list<(Set<int> * Set<int>)>) =
    let state = Dictionary<int, ResizeArray<int>>()
    let winnings =
        seq {
            for (num, card) in cards |> Seq.indexed do
                let cardNum = num + 1
                let outputs = treeScoreCard state cards cardNum
                yield (outputs |> Seq.length)
        }
        |> Seq.sum
    winnings

type Day04() =
    inherit Day()

    let samp = raw"""
        Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
        Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
        Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
        Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
        Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
        Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
        """
    override _.SolveA input =
        parse input
        |> Seq.map score
        |> Seq.sum 
        |> string

    override _.SolveB input =
        parse input
        |> List.ofSeq
        |> treeScore
        |> string

    override this.Tests =
        [
            Test("a", samp, "13", fun x -> this.SolveA x);
            Test("b", samp, "30", fun x -> this.SolveB x);
        ]
        |> List<Test>