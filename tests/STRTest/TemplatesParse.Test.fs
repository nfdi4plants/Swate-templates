module STRTest.TemplatesParse

open System.IO
open Expecto

open FsSpreadsheet
open FsSpreadsheet.Net

let Main = testList "Templates Parse" [

    for path, templateResult in TestData.TemplateResults do
        testCase path <| fun _ ->
            match templateResult with
            | Error (e) -> failwith $"Unable to read template: {path}. {e.Message}"
            | Ok _ ->
                // a bit redundant, i decided to use this syntax to improve error message in 'Error' case
                Expect.isOk templateResult $"Should be Ok for: '{path}'"
]

