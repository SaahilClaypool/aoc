namespace Aoc.Solutions.Y2023

[<AutoOpen>]
module Helpers =
    open Aoc.Runner
    let trace (value : 'a) : 'a =
        LogHelpers.Log<'a>(value) |> ignore
        value
    
    let json o =
        System.Text.Json.JsonSerializer.Serialize o

    let dump (value : 'a) : unit =
        LogHelpers.Log<string>(json value) |> ignore
