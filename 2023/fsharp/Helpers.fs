namespace Aoc.Solutions.Y2023

[<AutoOpen>]
module Helpers =
    open Aoc.Runner
    open System.Collections.Generic
    open System
    let lines (str: string) =
        str.Split '\n'
    let trace (value : 'a) : 'a =
        LogHelpers.Log<'a>(value) |> ignore
        value
    
    let json o =
        System.Text.Json.JsonSerializer.Serialize o

    let dump (value : 'a) : unit =
        LogHelpers.Log<string>(json value) |> ignore

    let lg (value : string) : unit =
        LogHelpers.Log<string>(value) |> ignore
    
    /// <summary>behave like c# raw string literal</summary>
    let raw (str: string) =
        // like c#, last line determines the depth
        let lines = str.Split '\n'
        let whitespace = Seq.last lines
        match whitespace with
        | leadingWhitespace when String.IsNullOrWhiteSpace leadingWhitespace ->
            lines
            |> Seq.skip 1
            |> Seq.map (fun line -> line[leadingWhitespace.Length..])
            |> Seq.fold (fun state next -> $"{state}\n{next}") ""
            |> (fun x -> x[1..(x.Length - 2)])
        | _ -> str

module Seq =
    let product (s : seq<int>) =
        s
        |> Seq.fold (fun state next -> state * next) 1

    let productL (s : seq<int64>) =
        s
        |> Seq.fold (fun state next -> state * next) 1L
    
    let all (predictate: 'a -> bool) (s : seq<'a>) =
        (s |> Seq.filter (predictate >> not)) |> Seq.length = 0