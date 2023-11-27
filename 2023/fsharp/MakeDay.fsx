open System.IO
open System.Xml

let (|Int|_|) (str:string) =
    match System.Int32.TryParse str with
    | true,int -> Some int
    | _ -> None

let make_day day path =
    printfn $"Making {day} - {path}"
    let (output, num) =
        match day with
        | Int i -> ($"{path}/Day%02d{i}.fs", $"%02d{i}")
        | _ -> ($"{path}/{day}", day)

    let template = (File.ReadAllText $"{path}/template.txt").Replace("{DD}", num)
    if File.Exists(output)  then
        $"File exists: {output}"
    else
        File.WriteAllText(output, template)
        let fspath = $"{path}/fsharp.fsproj"
        let fsproj = File.ReadAllText fspath
        let doc = new XmlDocument() in
            doc.LoadXml fsproj
        let itemGroup = doc.SelectSingleNode "(//ItemGroup)[1]"
        let compileEl = doc.CreateElement "Compile" in
            compileEl.SetAttribute("Include", Path.GetFileName output)
        itemGroup.InsertBefore(compileEl, itemGroup.LastChild) |> ignore
        let input = $"{path}/../inputs/Day_{num}.txt"
        if not (File.Exists input) then
            (File.CreateText input).Dispose |> ignore

        doc.Save fspath
        output


match fsi.CommandLineArgs with
| [| _; day |] -> make_day day "."
| [| _; day; path |] -> make_day day path
| _ -> failwith "provide a day"
|> fun x -> printfn $"{x}"
