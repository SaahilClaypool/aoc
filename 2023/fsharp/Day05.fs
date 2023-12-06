module Aoc.Solutions.Y2023.Day05

open Aoc.Runner
open System
open System.Text.RegularExpressions
open System.Collections.Generic

type Mapping = { src: int64; dst: int64; range: int64 }
type Amap = { sourceType: string; destType: string; mappings: seq<Mapping> }


let parse (text: string) =
    let lines = lines text |> List.ofSeq
    let seeds =
        Regex.Matches((Seq.head lines), @"\d+")
        |> Seq.map (fun m -> m.Value |> int64)
    
    let dicts = 
        seq {
            let mutable src = ""
            let mutable dst = ""
            let mutable d = List<Mapping>()
            for line in (Seq.skip 2 lines) do
                let m = Regex.Match(line, @"(\w+)-to-(\w+) map:")
                if m.Success then
                    src <- m.Groups[1].Value
                    dst <- m.Groups[2].Value
                    d <- List<Mapping>()
                elif String.IsNullOrWhiteSpace line then
                    let r = { sourceType = src; destType = dst; mappings = d }
                    yield r
                else
                    let nums = (line.Split ' ') |> Seq.map int64 |> Array.ofSeq
                    match nums with 
                    | [|destVal; srcVal; rangeVal|] ->
                        d.Add { src = srcVal; dst = destVal; range = rangeVal }
                    | _ -> ()
            yield { sourceType = src; destType = dst; mappings = d } 
        }
    
    (seeds, dicts)



let sample = raw"""
seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4
"""

let inRange num (mapping: Mapping) =
    num >= mapping.src && num <= mapping.src + mapping.range

let mapNumber (mapper: Amap) num =
    let m =
        match (mapper.mappings |> Seq.tryFind (inRange num)) with
        | Some mapping -> num - (mapping.src) + (mapping.dst)
        | None -> num
    
    lg $"{num} -> {m}"
    m

type range = (int64* int64)

// input 1 - 2 
// src       2 - 3
let intersection ((l1, r1): range) ((l2, r2): range) =
    if (l1 <= r2 && r1 >= l2) || (r1 >= l2 && l1 <= r2) then
        let leftmax = Math.Max(l1, l2)
        let rightmin = Math.Min(r1, r2)
        Some (leftmax, rightmin)
    else
        None

let exclusion ((l1, r1): range) ((l2, r2): range) =
    seq {
        if l1 = l2 && r1 = r2 then
            ()
        elif l1 < l2 && r1 > r2 then
            yield (l1, l2 - 1L)
            yield (r2 + 1L, r1)
        elif l1 < l2 then
            yield (l1, Math.Min(r1, l2 - 1L))
        elif (r1 > r2) then
            yield (Math.Max(l1, r2 + 1L), r1)
        else
            ()
    }

let mapperToRange m = (m.src, m.src + m.range - 1L)
let mapRange (mapper: Amap) (r: range) =
    let matchingRanges = 
        mapper.mappings
        |> Seq.choose (fun m ->
            match intersection r (mapperToRange m) with
            | Some inter ->
                Some((fst inter) - m.src + m.dst, (snd inter) - m.src + m.dst)
            | None -> None
        )
        |> List.ofSeq
    
    let unmatchedRange =
            mapper.mappings
            |> Seq.fold (fun state next ->
                state
                |> Seq.map 
                    (fun excluded ->
                        exclusion excluded (mapperToRange next)
                    )
                |> Seq.concat
            ) [r]
            |> List.ofSeq
    lg $"{mapper.destType} converting range {r} to {matchingRanges}"
    lg $"excluded: {unmatchedRange} (leaving as is)"
    Seq.concat [matchingRanges; unmatchedRange]

let mapRanges (mapper: Amap) (r: seq<range>) =
    let r = r |> List.ofSeq
    lg $"mapping {r} to with {mapper}"
    r
    |> Seq.map (mapRange mapper)
    |> Seq.concat


type Day05() =
    inherit Day()

    override _.SolveA input =
        let (seeds, amaps) = input |> parse
        let mapSeed (seed: int64) =
            amaps
            |> Seq.fold
                (fun state next ->
                    lg $"{next.destType}"
                    mapNumber next state) seed
        
        let finalValues =
            seeds
            |> Seq.map mapSeed

        finalValues |> Seq.min |> string

    override _.SolveB input =
        let (seeds, amaps) = input |> parse
        let seedRanges =
            Seq.chunkBySize 2 seeds
            |> Seq.map (fun l -> (l[0], l[0] + l[1] - 1L))

        // range -> intersections with all the next ranges
        // 1 - 5 | 1 - 3 5 - 7 -> 1-3  5-6
        let mapSeedRange (r: range) =
            amaps
            |> Seq.fold
                (fun state next ->
                    mapRanges next state) [r]
        let finalValues =
            seedRanges
            |> Seq.map mapSeedRange
            |> Seq.concat
            |> Seq.toList
        
        dump finalValues

        finalValues
        |> Seq.map fst
        |> Seq.min
        |> string
    
    override this.Tests =
        [
            Test("a", sample, "35", fun x -> this.SolveA x);
            Test("b", sample, "46", fun x -> this.SolveB x);
            // Test("x", "", "", fun x ->
            //     (exclusion (14L, 49L) (15L, 15L + 37L))
            //     |> Seq.toList
            //     |> string);
            // Test("z", sample.Replace("seeds: 79 14 55 13", "seeds: 55 56 57 58 59 60 61 62 63 64 65 66 67 68"), "?", fun x -> this.SolveA x);
        ]
        |> List<Test>