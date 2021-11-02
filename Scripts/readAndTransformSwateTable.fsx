#r "nuget: FSharpSpreadsheetML"
#r "nuget: FSharpAux"

open FSharpSpreadsheetML
open FSharpAux
open System

module String = 
    let splitS (delimiter : string) (str : string) = str.Split ([|delimiter|],StringSplitOptions.None)

/// Converts numbers to letters like the column keys in MS Excel.
let inline toExcelLetters number =
    if int number < 1 then failwith "ERROR: Only numbers > 0 can be converted to Excel letters."
    let rec loop no list =
        if no > 26. then
            loop 
                (if no % 26. = 0. then no / 26. - 0.1 else no / 26.) 
                (if no % 26. = 0. then 'Z'::list else (no % 26. + 64. |> char)::list)
        else
            if no % 26. = 0. then 'Z'::list else (no % 26. + 64. |> char)::list
    loop (float number) []
    |> System.String.Concat




let userProfile = Environment.SpecialFolder.UserProfile |> Environment.GetFolderPath
let filepath = userProfile + "/onedrive/csb-stuff/nfdi/template-skripts/1spl01_plants.xlsx"

let ss = Spreadsheet.fromFile filepath false

let oldSs = 
    IO.Path.Combine(userProfile, "onedrive/csb-stuff/nfdi/template-skripts/1spl01_plants_deprecated.xlsx")
    |> fun s -> Spreadsheet.saveAs s ss 

Spreadsheet.close oldSs

let sst = Spreadsheet.getSharedStringTable ss

let swateTables, wsps =
    let wbp = Spreadsheet.getWorkbookPart ss
    WorkbookPart.getWorkSheetParts wbp
    |> Seq.choose (
        fun t -> 
            let tab = Table.tryGetByNameBy (String.contains "annotationTable") t
            if tab.IsSome then Some (tab, t) else None
    )
    |> Seq.unzip
    |> fun (st,wsps') -> 
        Seq.map Option.get st |> Array.ofSeq, 
        Array.ofSeq wsps'

let sds = wsps |> Array.map Worksheet.WorksheetPart.getSheetData

let headersAll = 
    let getHeaders sd table = 
        let v = Table.getArea table
        let name = Table.getName table
        let row = Table.Area.upperBoundary v
        let colL, colR = Table.Area.leftBoundary v, Table.Area.rightBoundary v
        name,
        Array.init (int colR - int colL) (
            fun i -> 
                SheetData.getCellAt row (colL + uint i) sd 
                |> fun c -> 
                    match Cell.tryGetValue (Some sst) c with
                    | Some b    -> b
                    | None      -> ""
        )
    (sds, swateTables) ||> Array.map2 getHeaders

// TO DO: transfer stuff in first value row

let transformHeader header = 
    //let transformHeader header =
        //let headerTransformMap =
        //    Map {
        //        "(#h; #t"       , "("
        //        "(#%i; #h; #t"  , "("
        //        "; #u)"         , ")"
        //    }
    let createTsrAnew oldTsr =
        let newTsr = String.splitS "#t" oldTsr |> Array.item 1
        sprintf "Term Source Ref (%s" newTsr
    let createTanAnew oldTan =
        let newTan = String.splitS "#t" oldTan |> Array.item 1
        sprintf "Term Accession Number (%s" newTan
    match header with
        | x when String.contains "Term Source REF"          x -> createTsrAnew header
        | x when String.contains "Term Accession Number"    x -> createTanAnew header
        | x when String.contains "Unit"                     x -> "Unit"
        | _                                                   -> header

let transformedHeadersAll = 
    headersAll
    |> Array.map (
        fun (name,values) -> 
            values
            |> Array.choose (
                fun v ->
                    if String.contains "#u" v && String.contains "Unit" v |> not then
                        None
                    else Some (transformHeader v)
            )
    )

let createNewSwateTable name (transformedHeaders : ) =
    let area = 
        let firstRow = 1
        let lastRow = 2
        let firstColumn = "A"
        let lastColumn = toExcelLetters transformedHeaders.Length
        Table.Area.ofBoundaries
    Table.create name 


Spreadsheet.close ss