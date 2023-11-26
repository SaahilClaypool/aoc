namespace Aoc.Solutions.Y2023

[<AutoOpen>]
module Helpers =
    open Aoc.Runner
    let trace (value : 'a) : 'a =
        LogHelpers.Log<'a>(value) |> ignore
        value