#r "nuget: FSharpSpreadsheetML"
#r "nuget: FSharpAux"
#r "nuget: Newtonsoft.Json"

open FSharpSpreadsheetML
open DocumentFormat.OpenXml.Spreadsheet
open FSharpAux
open Newtonsoft.Json
open System
open System.IO

// ------------------------------------------------------------------------------------------------
// Types
// ------------------------------------------------------------------------------------------------

type CVEntry = {
    Ontology        : string
    TAN             : string
    TermSourceRef   : string
}

type TemplateMetadata = {
    Name        : string
    version     : string
    author      : string []
    description : string
    er          : string []
    tags        : string []
    Workbook    : string
    Worksheet   : string
    Table       : string
}

// ------------------------------------------------------------------------------------------------
// Functions
// ------------------------------------------------------------------------------------------------

// delete when pushed to F#Aux
module Array =
    /// Returns a new array containing only the elements of the input array for which the given predicate returns true.
    let filteri (predicate : int -> 'T -> bool) (array : 'T []) =
        let mutable i = -1
        Array.filter (
            fun x ->
                i <- i + 1
                predicate i x
        ) array
    /// Returns the length of an array containing only the elements of the input array for which the given predicate returns true.
    let filterLength (predicate : 'T -> bool) (array : 'T []) =
        let mutable counter = 0
        for i = 0 to array.Length - 1 do 
            if predicate array.[i] then counter <- counter + 1
        counter
    /// Returns the length of an array containing only the elements of the input array for which the given predicate returns true.
    let filteriLength (predicate : int -> 'T -> bool) (array : 'T []) =
        let mutable counter = 0
        for i = 0 to array.Length - 1 do 
            if predicate i array.[i] then counter <- counter + 1
        counter
    /// Applies the given function to each element of the array. Returns the array comprised of the results x for each element where the function returns Some x.
    let choosei (chooser: int -> 'T -> 'U Option) (array : 'T []) =
        checkNonNull "array" array    
        let inline subUnchecked startIndex count (array : _ []) =
            let res = Array.zeroCreate count
            if count < 64 then 
                for i = 0 to res.Length - 1 do res.[i] <- array.[startIndex + i]
            else Array.Copy (array, startIndex, res, 0, count)
            res
        let mutable i = 0
        let mutable first = Unchecked.defaultof<'U>
        let mutable found = false
        while i < array.Length && not found do
            let element = array.[i]
            match chooser i element with 
            | None -> i <- i + 1
            | Some b -> 
                first <- b
                found <- true                            
        if i <> array.Length then
            let chunk1 : 'U [] = Array.zeroCreate ((array.Length >>> 2) + 1)
            chunk1.[0] <- first
            let mutable count = 1            
            i <- i + 1                                
            while count < chunk1.Length && i < array.Length do
                let element = array.[i]                                
                match chooser i element with
                | None -> ()
                | Some b -> 
                    chunk1.[count] <- b
                    count <- count + 1                            
                i <- i + 1
            if i < array.Length then                            
                let chunk2 : 'U [] = Array.zeroCreate (array.Length-i)                        
                count <- 0
                while i < array.Length do
                    let element = array.[i]                                
                    match chooser i element with
                    | None -> ()
                    | Some b -> 
                        chunk2.[count] <- b
                        count <- count + 1                            
                    i <- i + 1
                let res : 'U [] = Array.zeroCreate (chunk1.Length + count)
                Array.Copy (chunk1, res, chunk1.Length)
                Array.Copy (chunk2, 0, res, chunk1.Length, count)
                res
            else subUnchecked 0 count chunk1                
        else Array.empty
    /// Returns an array with the indeces of the elements in the input array that satisfy the given predicate.
    let findIndeces (predicate : 'T -> bool) (array : 'T []) =
        let mutable counter = 0
        for i = 0 to array.Length - 1 do if predicate array.[i] then counter <- counter + 1
        let mutable outputArr = Array.zeroCreate counter
        counter <- 0
        for i = 0 to array.Length - 1 do if predicate array.[i] then outputArr.[counter] <- i; counter <- counter + 1
        outputArr
    /// Returns a reversed array with the indeces of the elements in the input array that satisfy the given predicate.
    let findIndecesBack (predicate : 'T -> bool) (array : 'T []) =
        let mutable counter = 0
        for i = 0 to array.Length - 1 do if predicate array.[i] then counter <- counter + 1
        let mutable outputArr = Array.zeroCreate counter
        counter <- 0
        for i = array.Length - 1 downto 0 do
            if predicate array.[i] then 
                outputArr.[counter] <- i
                counter <- counter + 1
        outputArr
    /// Returns an array comprised of every nth element of the input array.
    let takeNth (n : int) (array : 'T []) = filteri (fun i _ -> (i + 1) % n = 0) array
    /// Returns an array without every nth element of the input array.
    let skipNth (n : int) (array : 'T []) = filteri (fun i _ -> (i + 1) % n <> 0) array

// delete when pushed to F#Aux
module String =
    /// Returns the first char of a string.
    let first (str : string) = str.Chars 0
    /// Returns the last char of a string.
    let last (str : string) = str.Chars (str.Length - 1)
    /// Splits an input string at a given delimiter (substring).
    let splitS (delimiter : string) (str : string) = str.Split ([|delimiter|],StringSplitOptions.None)
    /// Returns the last index of a char in a string.
    let findIndexBack (ch : char) (str : string) = str.ToCharArray () |> Array.findIndexBack (fun c -> c = ch)
    /// Returns the first index of a char in a string.
    let findIndex (ch : char) (str : string) = str.ToCharArray () |> Array.findIndex (fun c -> c = ch)
    /// Returns the indeces of a char in a string.
    let findIndeces (ch : char) (str : string) = str.ToCharArray () |> Array.findIndeces (fun c -> c = ch)
    /// Returns the indeces of a char in a string sorted backwards.
    let findIndecesBack (ch : char) (str : string) = str.ToCharArray () |> Array.findIndecesBack (fun c -> c = ch)
    /// Iterates through the string and returns a string with the chars of the input until the predicate returned false the first time.
    let takeWhile (predicate : char -> bool) (str : string) = 
        if String.IsNullOrEmpty str then str
        else
            let mutable i = 0
            while i < str.Length && predicate str.[i] do i <- i + 1
            String.take i str
    /// Iterates through the string and returns a string that starts at the char of the input where the predicate returned false the first time.
    let skipWhile (predicate : char -> bool) (str : string) =
        if String.IsNullOrEmpty str then str
        else
            let mutable i = 0
            while i < str.Length && predicate str.[i] do i <- i + 1
            String.skip i str

let getCVEntry (s : string) =
    let tsr = 
        String.splitS "#t" s 
        |> Array.item 1 
        |> String.split ';'
        |> Array.head
        |> String.split ')'
        |> Array.head
    let onto = String.split ':' tsr |> Array.head
    let tan = String.split ':' tsr |> Array.item 1
    {
        Ontology        = onto
        TAN             = tan
        TermSourceRef   = tsr
    }

let toNumber (str : string) =
    let chArr = str.ToCharArray()
    let mutable i = chArr.Length |> float
    chArr
    |> Array.fold (
        fun acc v -> 
            i <- i - 1.
            acc + ((float v - 64.) * 26. ** i)
    ) 0.
    |> int

let isHeaderCell (table : Table) cell =
    // header area
    let v = (Table.getArea table)
    let row = Table.Area.upperBoundary v |> int
    let colL, colR = Table.Area.leftBoundary v |> int, Table.Area.rightBoundary v |> int
    //cell reference
    let cRow, cCol = 
        let chArr = (Cell.getReference cell).ToCharArray()
        chArr
        |> Array.takeWhile Char.IsLetter
        |> String
        |> toNumber,
        chArr
        |> Array.skipWhile Char.IsLetter
        |> String
        |> int
    cRow = row && cCol >= colL && cCol <= colR

// ------------------------------------------------------------------------------------------------
// Testing
// ------------------------------------------------------------------------------------------------

let dirs =
    let templDir = Path.Combine(__SOURCE_DIRECTORY__, "../templates")
    Directory.GetDirectories templDir

let templates =
    let files = dirs |> Array.collect (fun d -> Directory.GetFiles(d,"*.xlsx")) 
    files |> Array.map (fun f -> Spreadsheet.fromFile f false) // Open files

templates |> Array.iter (fun d -> d.Close()) // Close files

let outerJsons = // name is "outerJsons" because they are located outside of the .xlsx file
    let files = dirs |> Array.collect (fun d -> Directory.GetFiles(d,"*.json")) 
    files |> Array.map File.ReadAllText

//let testTemplate = templates.[0]
let testTemplate =
    let userProfile = Environment.SpecialFolder.UserProfile |> Environment.GetFolderPath
    userProfile + "/onedrive/csb-stuff/nfdi/template-skripts/1spl01_plants.xlsx"
    |> Spreadsheet.fromFile // Open file
    <| false

testTemplate.Close() // Close file

let testOuterJson =
    let userProfile = Environment.SpecialFolder.UserProfile |> Environment.GetFolderPath
    try
        userProfile + "/onedrive/csb-stuff/nfdi/template-skripts/1spl01_plants.json"
        |> File.ReadAllText
        |> JsonConvert.DeserializeObject<TemplateMetadata>
        |> Option.Some
    with :? FileNotFoundException -> None

let sst = Spreadsheet.getSharedStringTable testTemplate
let wbp = Spreadsheet.getWorkbookPart testTemplate
    
let testInnerTempMetadata =
    WorkbookPart.getWorkSheetParts wbp
    |> Seq.tryPick (
        fun t -> 
            let sd = Worksheet.get t |> Worksheet.getSheetData
            let check =
                SheetData.tryGetCellValueAt (Some sst) 7u 1u sd = Some "Table" && 
                SheetData.tryGetCellValueAt (Some sst) 7u 2u sd <> None
            if check then 
                let svm =
                    Worksheet.get t
                    |> Worksheet.getSheetData
                    |> SheetData.toSparseValueMatrix sst 
                Some svm
            else None
    )


// ++++++++++++++++ Spreadsheet-Tests ++++++++++++++++

let tryGetRowValues (sst : SharedStringTable option) (row : Row) =
    Row.toCellSeq row
    |> Seq.map (Cell.tryGetValue sst)

let getPresentRowValues (sst : SharedStringTable option) (row : Row) =
    Row.toCellSeq row
    |> Seq.choose (Cell.tryGetValue sst)

let sheetData = WorkbookPart.getWorkSheetParts wbp |> Seq.head |> Worksheet.get |> Worksheet.getSheetData
SheetData.getRows sheetData
|> Array.ofSeq
|> Array.map (fun row ->
    getPresentRowValues (Some sst) row |> Array.ofSeq
)

// +++++++++++++++++++++++++++++++++++++++++++++++++++


let annoTableMetadata =
    match (testOuterJson,testInnerTempMetadata) with
    | (Some md,None)    -> md.Table
    | (None,Some md)    -> md.Item (7,2)
    | (Some _, Some _)  -> failwith "ERROR: Template metadata in both versions (JSON, Worksheet) existing."
    | (None,None)       -> failwith "ERROR: No template metadata existing."

let wspSwateTable, swateTable =
    let wsp =
        WorkbookPart.getWorkSheetParts wbp
        |> Seq.find (fun t -> (Table.tryGetByNameBy ((=) "annotationTable") t).IsSome)
    wsp,
    Table.tryGetByNameBy ((=) "annotationTable") wsp
    |> Option.get

let sd = Worksheet.get wspSwateTable |> Worksheet.getSheetData

let headerArea =
    let v = (Table.getArea swateTable)
    let row = Table.Area.upperBoundary v
    let colL, colR = Table.Area.leftBoundary v, Table.Area.rightBoundary v
    Array.init (int colR - int colL) (fun i -> SheetData.getCellAt row (colL + uint i) sd)

let headerAreaValues = headerArea |> Array.map (Cell.getValue (Some sst))

let headers, tans = 
    let tanKey = "Term Accession Number "
    headerAreaValues 
    |> fun arr ->
        Array.filter (fun (t : string) -> not (t.Contains "#")) arr,
        Array.choose (fun (t : string) -> if t.Contains tanKey then Some (getCVEntry t) else None) arr

let ers =
    let rowL = SheetData.getMaxRowIndex sd
    let col1 = 
        Array.init (int rowL) (
            fun i ->
                match SheetData.tryGetCellValueAt (Some sst) (i + 1 |> uint) 1u sd with
                | Some a    -> a
                | None      -> ""
        )
    col1
    |> Array.choose (fun t -> if t.Contains("ER ") then Some (t.Remove(0, 3)) else None)
    |> Array.distinct

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

// TO DO: Vesuchen, irgendwie die SheetID aus dem WorksheetPart zu bekommen, sodass am Ende SheetName und Worksheet(Part) gematcht werden können.
// Ist wichtig, um später Sheets herstellen und mit den entsprechenden hergestellten Worksheet(Part)s verknüpfen zu können.
let matchSheetIDWithWorksheet =
    wspSwateTable
    |> Worksheet.WorksheetPart.get

testTemplate
|> Spreadsheet.getWorkbookPart
|> fun wbp ->
    Workbook.get wbp
    |> Sheet.Sheets.get
    |> Sheet.Sheets.getSheets
    |> Seq.head
    |> Sheet.getName
    WorkbookPart.getWorkSheetParts wbp
    |> Worksheet.WorksheetPart.getByID 
    |> Worksheet.get
    |> Worksheet.getSheetData
    |> Sheet.

testTemplate.
|> Spreadsheet.

// zu Funktion machen
let erSheet = 
    let 
    Sheet.create
    

// zu Funktion machen
let emptyErTable =
    let arr2d = 
        Array2D.init (headers.Length + 1) (combinedCols.Length + 1) (
            fun iR iC ->
                match (iR,iC) with
                | (0,0) -> ""
                | (0,_) -> combinedCols.[iC - 1]
                | (_,0) -> headers.[iR - 1]
                | (_,1) -> tans.[iR - 1].TermSourceRef
                | (_,2) -> tans.[iR - 1].Ontology
                | (_,3) -> tans.[iR - 1].TAN
                | (_,_) -> ""
        )


// zu Funktion machen
let fullErTable =