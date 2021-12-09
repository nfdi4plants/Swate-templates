// ------------------------------------------------------------------------------------------------
// References
// ------------------------------------------------------------------------------------------------

#r "nuget: FSharpAux"
#r "nuget: FSharpSpreadsheetML"

open FSharpAux
open FSharpSpreadsheetML
open DocumentFormat.OpenXml
open DocumentFormat.OpenXml.Packaging
open DocumentFormat.OpenXml.Spreadsheet
open System



// ------------------------------------------------------------------------------------------------
// Types
// ------------------------------------------------------------------------------------------------

type CvEntry = {
    Ontology            : string
    TermSourceRef       : string
    TermAccessionNumber : string
}

type Template = {
    Path        : string
    Name        : string
    SSDocument  : SpreadsheetDocument
    IsEditable  : bool
}

type IsaType =
| Parameter
| Characteristics
| Factor
| SampleName
| SourceName
| DataFileName

type BuildingBlockHeader = {
    IsaTypeColumnHeader : IsaType
    TSRColumnHeader     : string option
    TANColumnHeader     : string option
    UnitHeader          : string option
}



// ------------------------------------------------------------------------------------------------
// Values
// ------------------------------------------------------------------------------------------------

let cvEntryCols = [|
    "TermSourceRef"
    "Ontology"
    "TAN"
|]

let validationCols = [|
    "Content type (validation)"
    "Notes during templating"
|]

let erCols = [|
    "Target term"
    "Instruction"
    "Requirement (m/o/n)"
    "Value (cv/s/d)"
    "Additional information"
    "Review comments"
|]

let combinedCols = Array.concat [|cvEntryCols; validationCols; erCols|]



// ------------------------------------------------------------------------------------------------
// Functions
// ------------------------------------------------------------------------------------------------

let emptyCvEntry () = {
    Ontology            = String.Empty
    TermSourceRef       = String.Empty
    TermAccessionNumber = String.Empty
}

/// Takes a Swate table columns and returns its 
let toNonHiddenColumn col =
    let hasWord word = String.contains word col = true
    match col with
    | _ when hasWord "Parameter ["          -> Parameter
    | _ when hasWord "Factor ["             -> Factor
    | _ when hasWord "Characteristics ["    -> Characteristics
    | _ when hasWord "Sample Name"          -> SampleName
    | _ when hasWord "Source Name"          -> SourceName
    | _ when hasWord "Data File Name"       -> DataFileName
    | _                                     -> failwith "ERROR: Improper word given."

/// Checks if a columns is a non-hidden Swate table column.
let isNonHiddenColumn col = (try Some (toNonHiddenColumn col) with _ -> None).IsSome

/// Transforms a BuildingBlockHeader to a CvEntry.
let toCvEntry buildingBlockHeader =
    let isUserSpecific col = String.contains "()" col
    let tsrCol = buildingBlockHeader.TSRColumnHeader.Value
    match tsrCol with
    | x when isUserSpecific x = true -> 
        {
            Ontology            = "user-specific"
            TermSourceRef       = "user-specific"
            TermAccessionNumber = "user-specific"
        }
    | _ ->
        let tsr = 
            String.splitS "Term Source REF (" tsrCol
            |> Array.item 1 
            |> String.split ')'
            |> Array.head
        let onto = String.split ':' tsr |> Array.head
        let tan = 
            let purlLink = @"http://purl.obolibrary.org/obo/"
            let underscoreTsr = String.replace ":" "_" tsr
            let uri = purlLink + underscoreTsr
            uri
        {
            Ontology            = onto
            TermSourceRef       = tsr
            TermAccessionNumber = tan
        }

