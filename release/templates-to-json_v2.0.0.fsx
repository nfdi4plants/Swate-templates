#r "nuget: ARCtrl.NET, 2.0.0-alpha.2"
#r "nuget: FsSpreadsheet, 6.1.3"

open System.IO
open FsSpreadsheet
open FsSpreadsheet.Net
open ARCtrl.Template
open ARCtrl.NET

let version = "2.0.0"

let source0 = Path.GetFullPath(__SOURCE_DIRECTORY__)
let source = Directory.GetParent(source0).FullName
let inputPath = Path.Combine(source, "templates")
let outputPath = Path.Combine(source, "templates-to-json")

let outputFileName = Path.Combine(outputPath, $"templates_v{version}.json")
let reportFileName = Path.Combine(outputPath, $"report_v{version}.txt")


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

let ensureDirectory (dirPath : string) =
    if Directory.Exists (dirPath) |> not then
        Directory.CreateDirectory (dirPath) |> ignore

ensureDirectory outputPath

let log = Logger(reportFileName)

log.Info("Starting templates-to-json.fsx")


log.Info(sprintf "Get file paths in %s" inputPath)
let files = Path.getAllFilePaths inputPath

log.Info(sprintf "Found %d files" files.Length)

let xlsxFiles = files |> Array.filter (fun f -> f.EndsWith(".xlsx"))

log.Info(sprintf "Found %d xlsx files" xlsxFiles.Length)

open ARCtrl.Spreadsheet



let templates = 
    xlsxFiles
    |> Array.choose (fun f -> 
        try 
            let p = inputPath + f
            FsWorkbook.fromXlsxFile p
            |> Template.fromFsWorkbook
            |> fun wb -> Some (wb)
        with
        | ex -> 
            log.Error(sprintf "Error loading template %s: %s" f ex.Message)
            None
    )

log.Info(sprintf "Success! Read %d templates" templates.Length)

open ARCtrl.Json

let json = 
    templates 
    |> Templates.toJsonString 0

log.Info("Write json")

File.WriteAllText(outputFileName, json)

log.Info("Finished templates-to-json.fsx")
