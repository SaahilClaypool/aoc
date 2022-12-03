module Aoc.Solutions.Day03

open Aoc.Runner

let toInt number =
    let str =
        number
        |> List.map (fun x -> if x then '1' else '0')
        |> fun x -> (System.String.Concat(Array.ofList(x)))


    System.Convert.ToInt32(str, 2)

let parse (input: string) =
    input.Split('\n')
    |> List.ofSeq
    |> List.map (fun x ->
        x
        |> Seq.map (fun c ->
            if c = '1' then true else false)
        |> List.ofSeq)

let mostCommon (nums: list<list<bool>>) i =
    let numsInPos =
        nums
        |> List.map (fun n ->
            n[i]
        )
    let num1 =
        (numsInPos
        |> List.filter (fun x -> x)).Length
    num1 >= (nums.Length + 1) / 2

let leastCommon (nums: list<list<bool>>) i =
    let numsInPos =
        nums
        |> List.map (fun n ->
            n[i]
        )
    (numsInPos
    |> List.filter (fun x -> x)).Length < (nums.Length + 1) / 2

type Day03() =
    inherit Day()

    override _.SolveA input =
        let inp = parse input
        let len = inp[0].Length
        let solve fn =
            seq {
                for i in 0 .. (len - 1) ->
                    fn inp i
            } |> List.ofSeq
        let most = toInt (solve mostCommon)
        let least = toInt (solve leastCommon)
        (most * least) |> string


    override _.SolveB input =
        let inp = parse input
        let len = inp[0].Length
        let solve fn =
            seq {
                let mutable curNums = inp
                for i in 0 .. (len - 1) do
                    let num = fn curNums i
                    curNums <-
                        curNums
                        |> Seq.filter (fun n -> n[i] = num)
                        |> Seq.map List.ofSeq
                        |> List.ofSeq
                    if curNums.Length = 1 then
                        let num = curNums[0]
                        yield num
            }
            |> Seq.exactlyOne
        let a = solve mostCommon |> toInt
        let b = solve leastCommon |> toInt
        (a * b) |> string

    override this.Tests =
        [
            new Test(
                "B",
               @"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010",
            "230",
            fun x -> this.SolveB(x)
            )
        ]
        |> ResizeArray
