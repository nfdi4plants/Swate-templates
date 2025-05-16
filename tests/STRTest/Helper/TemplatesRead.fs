module STRTest.ReadTemplates

open System
open System.IO

open ARCtrl
open FsSpreadsheet
open FsSpreadsheet.Net


let getAllXLSXFilePaths() =
    let directories = DirectoryInfo(Constants.TemplateFolderPath).GetDirectories()

    let filePaths =
        directories
        |> Array.collect(fun directory ->
            directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
        |> Array.map (fun file -> file.FullName)
    filePaths


let tryReadTemplate (templatePath: string) =
    let result =
        try
            templatePath
            |> FsSpreadsheet.FsWorkbook.fromXlsxFile
            |> Spreadsheet.Template.fromFsWorkbook
            |> Ok
        with
        | e -> Error(e)
    templatePath, result

let readTemplate (templatePath: string) =
    tryReadTemplate templatePath
    |> function
        | p, Error (e) -> failwith $"Unable to read template: {p}. {e.Message}"
        | _,Ok template -> template

let readAllTemplates() =
    getAllXLSXFilePaths()
    |> Array.map readTemplate

let tryReadTemplates() =
    getAllXLSXFilePaths()
    |> Array.map tryReadTemplate
