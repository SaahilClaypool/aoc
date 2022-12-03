module Aoc.Solutions.Day02

open Aoc.Runner

type Command = { dir: string; dist: int }
type Position = { depth: int; horz: int; aim: int}

let parse (line: string) =
    let parts = line.Split()
    { dir = parts[0]; dist = int parts[1] }

let parseInput (input: string) =
    input.Split('\n')
        |> Seq.map parse

let executeOne pos command =
    match command with
    | { dir = "forward"; dist = d  } -> { pos with horz = pos.horz + d }
    | { dir = "up"; dist = d  } -> { pos with depth = pos.depth - d }
    | { dir = "down"; dist = d  } -> { pos with depth = pos.depth + d }
    | _ -> failwith("Not implemented")


let executeAim pos command =
    match command with
    | { dir = "forward"; dist = d  } ->
        { pos with horz = pos.horz + d; depth = pos.depth + d * pos.aim }
    | { dir = "up"; dist = d  } -> { pos with aim = pos.aim - d }
    | { dir = "down"; dist = d  } -> { pos with aim = pos.aim + d }
    | _ -> failwith("Not implemented")


let execute fn inputs =
    inputs
    |> Seq.fold fn { depth = 0; horz = 0 ; aim = 0}

type Day02() =
    inherit Day()

    override _.SolveA(input) =
        let position =
            parseInput input
            |> execute executeOne
        string (position.depth * position.horz)

    override _.SolveB(input) =
        let position =
            parseInput input
            |> execute executeAim
        string (position.depth * position.horz)

    override this.Tests =
        [ new Test(
              name = "basic part A",
              input =
                  @"forward 5
down 5
forward 8
up 3
down 8
forward 2",
              expectedOutput = "150",
              solve = fun x -> this.SolveA(x)
         );
        new Test(
              name = "part B",
              input =
                  @"forward 5
down 5
forward 8
up 3
down 8
forward 2",
              expectedOutput = "900",
              solve = fun x -> this.SolveB(x)
          ) ]
        |> ResizeArray
