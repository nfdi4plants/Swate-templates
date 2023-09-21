#r "nuget: ARCtrl, 1.0.0-alpha6"
#r "nuget: FsSpreadsheet.ExcelIO, 4.1.0"

open System.IO
open FsSpreadsheet
open FsSpreadsheet.ExcelIO
open ARCtrl.Templates


let standardizeSlashes (path : string) = 
    path.Replace("\\","/") 

let source = __SOURCE_DIRECTORY__ 
let inputPath = Path.Combine(source, "templates")
let outputPath = Path.Combine(source, "templates-to-json")

let outputFileName = Path.Combine(outputPath, "templates.json")
let reportFileName = Path.Combine(outputPath, "report.txt")


type Logger(path : string) =
    
    let fileWriter = new StreamWriter(path)

    member this.Info(s) = 
        printfn "INFO: %s" s
        fileWriter.WriteLine(sprintf "INFO: %s" s)
        fileWriter.Flush()

    member this.Error(s) = 
        printfn "ERROR: %s" s
        fileWriter.WriteLine(sprintf "ERROR: %s" s)
        fileWriter.Flush()

let makeRelative directoryPath (path : string) = 
    if directoryPath = "." || directoryPath = "/" || directoryPath = "" then path
    else
        if path.StartsWith(directoryPath) then 
            path.Substring(directoryPath.Length)
        else path


let getAllFilePaths (directoryPath : string) =
    let rec allFiles dirs =
        if Seq.isEmpty dirs then Seq.empty else
            seq { yield! dirs |> Seq.collect Directory.EnumerateFiles
                  yield! dirs |> Seq.collect Directory.EnumerateDirectories |> allFiles }

    allFiles [directoryPath] |> Seq.toArray
    |> Array.map (makeRelative directoryPath)

let ensureDirectory (dirPath : string) =
    if Directory.Exists (dirPath) |> not then
        Directory.CreateDirectory (dirPath) |> ignore

ensureDirectory outputPath

let log = Logger(reportFileName)

log.Info("Starting templates-to-json.fsx")


log.Info(sprintf "Get file paths in %s" inputPath)
let files = getAllFilePaths inputPath

log.Info(sprintf "Found %d files" files.Length)

let xlsxFiles = files |> Array.filter (fun f -> f.EndsWith(".xlsx"))

log.Info(sprintf "Found %d xlsx files" xlsxFiles.Length)

let templates = 
    xlsxFiles
    |> Array.choose (fun f -> 
        try 
            let p = Path.Combine(inputPath, f.TrimStart('\\'))
            log.Info(sprintf "InputPath %s" inputPath)
            log.Info(sprintf "Loading template %s" p)
            FsWorkbook.fromXlsxFile p
            |> Spreadsheet.Template.fromFsWorkbook
            |> fun wb -> Some (f,wb)
        with
        | ex -> 
            log.Error(sprintf "Error loading template %s: %s" f ex.Message)
            None
    )

log.Info(sprintf "Loaded %d templates" templates.Length)

let json = 
    templates 
    |> Array.toList
    |> List.map (fun (p,t) -> p |> standardizeSlashes,Json.Template.encode t)
    |> Thoth.Json.Net.Encode.object

log.Info("Write json")

File.WriteAllText(outputFileName, json.ToString())

log.Info("Finished templates-to-json.fsx")
