#r "nuget: ARCtrl.NET, 1.0.0-alpha4"
#r "nuget: Expecto, 10.1.0"

open System.IO
open FsSpreadsheet
open FsSpreadsheet.ExcelIO
open ARCtrl.Template
open ARCtrl.NET

let source = __SOURCE_DIRECTORY__ 
let inputPath = Path.Combine(source, "templates")

let files = Path.getAllFilePaths inputPath

printfn "Found %d files" files.Length

let xlsxFiles = files |> Array.filter (fun f -> f.EndsWith(".xlsx"))

printfn "Found %d xlsx files" xlsxFiles.Length

open Expecto

let testTemplateFile (templatePath: string) = 
    testCase templatePath <| fun _ ->
        let p = inputPath + templatePath
        let template =
            try
                FsWorkbook.fromXlsxFile p
                |> Spreadsheet.Template.fromFsWorkbook
                |> fun wb -> Ok (wb)
            with
                | e -> Error(templatePath,e)
        match template with
        | Error (p,e) -> failwith $"Unable to read template: {p}. {e.Message}"
        | Ok _ ->
            // a bit redundant, i decided to use this syntax to improve error message in 'Error' case
            Expect.isOk template $"Should be Ok for: '{templatePath}'"
    

let tests = testList "TemplateConversion" [
    for file in xlsxFiles do
        testTemplateFile file
]

let result = Tests.runTestsWithCLIArgs [] [||] tests

match result with
| 0 -> printfn "All checks successfull! ✅"
| 1 -> failwith "Error! Tests errored!"
| 2 -> failwith "Error! Tests failed!"
| anyElse -> failwithf "Error! Unknown exit condition! %i" anyElse 