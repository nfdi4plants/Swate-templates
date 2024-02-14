#r "nuget: ARCtrl.NET, 1.0.4"
#r "nuget: Expecto, 10.1.0"

open System.IO
open FsSpreadsheet
open FsSpreadsheet.ExcelIO
open ARCtrl.Template
open ARCtrl.NET

let source = __SOURCE_DIRECTORY__ 
let inputPath = Path.Combine(source, "..", "templates")
let options = EnumerationOptions()
options.RecurseSubdirectories <- true
let files = Directory.GetFiles(inputPath,"*.xlsx",enumerationOptions=options)

printfn "Found %i .xlsx files!" files.Length

open Expecto

let testTemplateFile (templatePath: string) = 
    testCase templatePath <| fun _ ->
        let p = templatePath
        let template =
            try
                FsWorkbook.fromXlsxFile p
                |> Spreadsheet.Template.fromFsWorkbook
                |> Ok
            with
                | e -> Error(templatePath,e)
        match template with
        | Error (p,e) -> failwith $"Unable to read template: {p}. {e.Message}"
        | Ok _ ->
            // a bit redundant, i decided to use this syntax to improve error message in 'Error' case
            Expect.isOk template $"Should be Ok for: '{templatePath}'"
    

let tests = testList "TemplateConversion" [
    for file in files do
        testTemplateFile file
]

let result = Tests.runTestsWithCLIArgs [] [||] tests

match result with
| 0 -> 
  printfn "All checks successfull! ✅"
  System.Environment.ExitCode <- 0
  System.Environment.Exit(0)
  0
| 1 -> 
  System.Environment.ExitCode <- 1
  printfn "Error! Tests failed!"
  System.Environment.Exit(1)
  1
| 2 -> 
  System.Environment.ExitCode <- 2
  printfn "Error! Tests errored!"
  System.Environment.Exit(2)
  2
| anyElse -> failwithf "Error! Unknown exit condition! %i" anyElse 