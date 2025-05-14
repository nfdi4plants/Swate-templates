#r "nuget: ARCtrl.NET, 1.0.0"

open System.IO
open FsSpreadsheet
open FsSpreadsheet.ExcelIO
open ARCtrl.Template
open ARCtrl.NET

let source0 = Path.GetFullPath(__SOURCE_DIRECTORY__)
let source = Directory.GetParent(source0).FullName
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

let getLatestTemplate (templates: (string * Template) []) =
     templates
     |> Array.sortByDescending (fun (name, template) -> template.Version)
     //enables checking, whether the templates are 
     //|> Array.map (fun template -> 
     //    printfn "template.Name: %s template.Version: %s" template.Name template.Version
     //    template)
     |> Array.head

let templates = 
    xlsxFiles
    |> Array.choose (fun f -> 
        try 
            let p = inputPath + f
            FsWorkbook.fromXlsxFile p
            |> Spreadsheet.Template.fromFsWorkbook
            |> fun wb -> Some (f,wb)
        with
        | ex -> 
            log.Error(sprintf "Error loading template %s: %s" f ex.Message)
            None
    )

let latestTemplates =
    let groupedTemplates =
        templates
        |> Array.groupBy (fun (name, template) -> template.Id)
    groupedTemplates
    |> Array.map (fun (_, templates) -> 
        getLatestTemplate templates
    )

log.Info(sprintf "Success! Read %d templates" templates.Length)

let json = 
    latestTemplates 
    |> Json.Templates.toJsonString 2

log.Info("Write json")

File.WriteAllText(outputFileName, json)

log.Info("Finished templates-to-json.fsx")
