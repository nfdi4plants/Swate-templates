#r "nuget: FSharpSpreadsheetML"
#r "nuget: FSharpAux"

open FSharpSpreadsheetML
open FSharpAux
open System

module String = 
    let splitS (delimiter : string) (str : string) = str.Split ([|delimiter|],StringSplitOptions.None)

let userProfile = Environment.SpecialFolder.UserProfile |> Environment.GetFolderPath
let filepath = userProfile + "/onedrive/csb-stuff/nfdi/template-skripts/1spl01_plants.xlsx"

let ss = Spreadsheet.fromFile filepath false

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

Table.create


Spreadsheet.close ss