module Aoc.Solutions.Day06

open Aoc.Runner

let parse (input: string) =
    input.Split(',')
    |> Seq.map int64
    |> Seq.groupBy (fun x -> x)
    |> Seq.map (fun (key, values) -> (key, Seq.length values |> int64))

let nextState state =
    state
    |> Seq.map (fun (key, values) ->
        seq {
            if key = 0L then
                yield (6L, values)
                yield (8L, values)
            else
                yield (key - 1L, values)
        })
    |> Seq.concat
    |> Seq.groupBy (fun (key, _) -> key)
    |> Seq.map (fun (key, multValues) -> (key, multValues |> Seq.map (fun (k, v) -> v) |> Seq.sum))

type Day06() =
    inherit Day()

    override _.SolveA input =
        let state = parse input

        seq { 1L .. 80L }
        |> Seq.fold (fun cur _ -> nextState cur) state
        |> Seq.sumBy (fun (_, v) -> v)
        |> string

    override _.SolveB input =
        let state = parse input

        seq { 1L .. 256L }
        |> Seq.fold (fun cur _ -> nextState cur) state
        |> Seq.sumBy (fun (_, v) -> v)
        |> string
