module Aoc.Solutions.Y2023.Day07

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic

type Hand = list<int>
type Bid = (Hand * int)

type HandType =
| Five = 5
| Four = 4
| Three = 3
| TwoPair = 2
| OnePair = 1
| HighCard = 0

let handeType (hand: Hand) : HandType =
    let byNumber =
        hand
        |> Seq.groupBy (fun x -> x)
        |> Seq.map (fun x -> (fst x), (snd x ) |> Seq.length)
        |> Seq.sortByDescending snd
        |> List.ofSeq
    match (Seq.head byNumber |> snd) with
    | 5 -> HandType.Five
    | 4 -> HandType.Five
    | 3 -> HandType.Three
    | 2 ->
        match byNumber |> Seq.filter (fun z -> (fst z) = 2) |> Seq.length with
        | 2 -> HandType.TwoPair
        | _ -> HandType.OnePair
    | _ -> HandType.HighCard


let compareHand left right =
    if left = right then
        printfn $"{left} = {right}"
        -1 
    else
        let leftType = handeType left
        let rightType = handeType right
        if leftType <> rightType then
            leftType.CompareTo(rightType)
        else
            let (l, r) =
                seq {
                    for i in 0..Seq.length left do
                        let lv, rv  = left[i], right[i]
                        if lv <> rv then
                            yield lv, rv
                }
                |> Seq.head
                
            l.CompareTo(r)

let compareBid (left: Bid) (right: Bid) =
    compareHand (fst left) (fst right)

let parseLine (line: string) =
    let (left, right) = ((line.Split ' ')[0], (line.Split ' ')[1])
    let hand =
        left
        |> Seq.map (fun x ->
            match x with
            | 'A' -> 14
            | 'K' -> 13
            | 'Q' -> 12
            | 'J' -> 11
            | 'T' -> 10
            | _ -> int $"{x}")
            |> List.ofSeq
    (hand, int right): Bid

let parse (input: string) =
    lines input
    |> Seq.map parseLine
    |> List.ofSeq

type Day07() =
    inherit Day()

    override _.SolveA input =
        let bids = parse input |> ResizeArray
        
        let sorted =
            bids
            |> Seq.sortWith compareBid
            |> Seq.toList

        let ost =
            sorted
            |> Seq.fold (fun s x ->
                    let hand, bid = x
                    let line = $"{json hand} {bid} {handeType hand}"
                    $"{line}\n{s}"
                ) ""
        printfn $"{ost}"
        printfn $"{sorted |> Seq.map fst |> Seq.distinct |> Seq.length}"
        sorted
        |> Seq.indexed
        |> Seq.map (fun (i, bid) -> (int64 (snd bid)) * int64(i + 1))
        |> Seq.sum
        |> string


    override _.SolveB input =
        ""


    member _.samp = raw"""
    32T3K 765
    T55J5 684
    KK677 28
    KTJJT 220
    QQQJA 483
    """

    override this.Tests =
        [
            Test("a", this.samp, "6440", fun x -> this.SolveA x);
        ]
        |> List<Test>