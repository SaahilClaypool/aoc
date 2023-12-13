module Aoc.Solutions.Y2023.Day12

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic

let unfold (content: string) separator cnt =
    String.Join(separator, Seq.replicate cnt content)

let parseline replicateCount (line: string) =
    match line.Split ' ' with
    | [|left; right|] ->
        unfold left "?" replicateCount,
        (unfold right "," replicateCount).Split ',' |> Seq.map int |> Seq.toList
    | _ -> failwith "bad input"

let parse input replicateCount =
    input |> lines |> Seq.map (parseline replicateCount)

let rec solveLine (line: string) (nums: seq<int * int>) (cache: Dictionary<string, int64>) =
    let nums = Seq.cache nums
    if Seq.isEmpty nums && not(line.Contains('#')) then
        1L
    elif Seq.isEmpty nums then
        0L
    elif line.Length = 0 then
        0L
    else
        let (idx, contig) = Seq.head nums

        let cacheKey = $"{line}_{idx}_{contig} - {json nums}"
        lg cacheKey
        if cache.ContainsKey cacheKey then
            cache[cacheKey]
        else
            let combinations =
                match line[0] with
                | '#' -> consumeContigiousFailure line nums cache contig
                | '.' -> skipNonFailure line nums cache contig
                | '?' -> (consumeContigiousFailure line nums cache contig) + (skipNonFailure line nums cache contig)
                | _ -> failwith "bad char"
            cache.Add(cacheKey, combinations)
            cache[cacheKey]
and skipNonFailure line nums cache length =
    solveLine line[1..] nums cache
and consumeContigiousFailure (line: string) nums cache length =
    let tooShort = line.Length < length
    let hasWorkingSpring = line[..length - 1].Contains('.')
    let doesNotEnd =
        line.Length > length &&
        line[length] = '#'
    if tooShort || hasWorkingSpring || doesNotEnd then
        0
    else
        let mutable restLine = line[length..]
        if restLine.Length > 0 && restLine[0] = '?' then
            restLine <- $".{restLine[1..]}"
        solveLine restLine (nums |> Seq.tail) cache
        

type Day12() =
    inherit Day()

    override _.SolveA input =
        parse input 1
        |> Seq.map (fun x -> solveLine (fst x) ((snd x) |> Seq.indexed) (Dictionary<string, int64>()))
        |> Seq.sum
        |> string

    override _.SolveB input =
        parse input 5
        |> Seq.map (fun x -> solveLine (fst x) ((snd x) |> Seq.indexed) (Dictionary<string, int64>()))
        |> Seq.sum
        |> string
    override this.Tests =
        [
            // Test("a1", ".??..??...?##. 1,1,3", "4", fun x -> this.SolveA x);
            // Test("a2", "?#?#?#?#?#?#?#? 1,3,1,6", "1", fun x -> this.SolveA x);
            // Test("a3", "????.######..#####. 1,6,5", "4", fun x -> this.SolveA x);
            Test("a4", "?###???????? 3,2,1", "10", fun x -> this.SolveA x);
        ]
        |> List<Test>