#r "nuget: FSharpSpreadsheetML"

open FSharpSpreadsheetML

let templates =
    let dirs = System.IO.Directory.GetDirectories "../templates/"
    let files = dirs |> Array.collect (fun d -> System.IO.Directory.GetFiles(d,"*.xlsx")) 
    files |> Array.map (fun f -> Spreadsheet.fromFile f false)

templates |> Array.iter (fun d -> d.Close())

let testTemplate = templates.[0]

let swateTable =
    testTemplate
    |> Spreadsheet.getWorkbookPart
    |> WorkbookPart.getWorkSheetParts
    |> Seq.head
    |> Table.tryGetByNameBy ((=) "annotationTable")
    |> Option.get

swateTable
|> Table.getArea
