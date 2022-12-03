open Aoc.Runner;
open System;
// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"
AocRunner.Run(Array.tail(Environment.GetCommandLineArgs()));
