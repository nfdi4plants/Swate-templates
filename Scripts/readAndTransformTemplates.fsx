#r "nuget: FSharpAux"
#r "nuget: FSharpSpreadsheetML"
#r "nuget: Newtonsoft.Json"

open FSharpSpreadsheetML
open DocumentFormat.OpenXml
open DocumentFormat.OpenXml.Packaging
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

module JaggedArray =
    
    /// Creates a jagged array given the dimensions and a generator function to compute the elements.
    let init count1 count2 (initializer : int -> int -> 'T) =
        Array.init count1 (
            fun i -> 
                Array.init count2 (
                    fun j -> initializer i j
                )
        )

module JaggedList =
    
    /// Creates a jagged list given the dimensions and a generator function to compute the elements.
    let init count1 count2 (initializer : int -> int -> 'T) =
        List.init count1 (
            fun i -> 
                List.init count2 (
                    fun j -> initializer i j
                )
        )

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
    | (Some _,Some _)   -> failwith "ERROR: Template metadata in both versions (JSON, Worksheet) existing."
    | (None,None)       -> failwith "ERROR: No template metadata existing."

let wspSwateTable =
    WorkbookPart.getWorkSheetParts wbp
    |> Seq.find (fun t -> (Table.tryGetByNameBy ((=) "annotationTable") t).IsSome)
    
let swateTable = (Table.tryGetByNameBy ((=) "annotationTable") wspSwateTable).Value

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

Spreadsheet.saveAs @"C:\Users\Mauso\OneDrive\CSB-Stuff\NFDI\Template-Skripts\1SPL01_plants_bearbeitungsCopy.xlsx" testTemplate

let initNewErSheet ss er =
    let ss = Spreadsheet.fromFile @"C:\Users\Mauso\OneDrive\CSB-Stuff\NFDI\Template-Skripts\1SPL01_plants_bearbeitungsCopy.xlsx" true
    let er = ers.[0]

    // Get Workbookpart and Workbook
    let wbp = Spreadsheet.getWorkbookPart ss
    let wb = Workbook.get wbp
    
    // Create new WorksheetPart, Worksheet, and SheetData and connect them.
    let newWsp = WorkbookPart.initWorksheetPart wbp
    WorkbookPart.addWorksheetPart newWsp wbp |> ignore
    let newWs = Worksheet.empty ()
    Worksheet.setWorksheet newWs newWsp |> ignore
    let newSd = SheetData.empty ()
    Worksheet.addSheetData newSd newWs |> ignore

    // Get sheets object and numbers
    let sheets = Sheet.Sheets.get wb
    let currMaxSheetNo = 
        Sheet.Sheets.getSheets sheets
        |> Seq. (
            fun s -> s.Id.Value
        )
    let newErSheetNo = uint currMaxSheetNo + 1u

    // Create new sheet and add it to the sheets object
    let idOfNewWsp = WorkbookPart.getWorksheetPartID newWsp wbp
    let newSheet = Sheet.create idOfNewWsp er newErSheetNo
    sheets.AppendChild newSheet |> ignore

    /// Function to create cells on the basis of column index, row index, and a value (string).
    let createNewCell (colI : uint32) rowI v = 
        let dt = 
            let chArr = String.toCharArray v
            let onlyDigits = chArr |> Array.forall Char.IsDigit
            if onlyDigits then CellValues.Number else CellValues.String
        let cr = sprintf "%s%i" (toExcelLetters colI) rowI
        let cv = CellValue v
        Cell.create dt cr cv

    // Blank of the first row with all the columns
    let colRow =
        let r = Row.empty ()
        Row.setIndex 1u r |> ignore
        SheetData.appendRow r newSd |> ignore
        r
        
    // Associate all cells with the column row
    combinedCols
    |> Array.iteri (
        fun i c ->
            let newCell = createNewCell 1u (i + 1) c
            if i = 0 then () else Row.appendCell newCell colRow |> ignore
    )
    
    let otherRows =
        let l = headers.Length
        let rs =
            Array.init l (
                fun i ->
                    let r = Row.empty ()
                    let ri = i + 2
                    Row.setIndex (uint ri) r |> ignore
                    let headerCell  = createNewCell 1u ri headers.[i]
                    let ontoCell    = createNewCell 2u ri tans.[i].Ontology
                    let tanCell     = createNewCell 3u ri tans.[i].TAN
                    let tsrCell     = createNewCell 4u ri tans.[i].TermSourceRef
                    Row.appendCell headerCell   r |> ignore
                    Row.appendCell ontoCell     r |> ignore
                    Row.appendCell tanCell      r |> ignore
                    Row.appendCell tsrCell      r |> ignore
            )
        rs

    Spreadsheet.close ss; ()
    

/// Create a metadata sheet with a given name of an ER and add it to a given SpreadsheetDocument.
let initErSheet erSheetName (doc : SpreadsheetDocument) = 

    let emptyErTable =
        let jaggArr = 
            JaggedArray.init (headers.Length + 1) (combinedCols.Length + 1) (
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
        jaggArr

    let sheet = SheetData.empty ()

    emptyErTable
    |> Array.foldi (
        fun i s row ->
            Row.ofValues None (uint i + 1u) row
            |> fun r -> SheetData.appendRow r s
    ) sheet
    |> ignore

    doc
    |> Spreadsheet.getWorkbookPart
    |> WorkbookPart.appendSheet erSheetName sheet
    |> ignore 

    doc

Spreadsheet.saveAs @"C:\Users\Mauso\OneDrive\CSB-Stuff\NFDI\Template-Skripts\1SPL01_plants_bearbeitungsCopy.xlsx" testTemplate
let ss = Spreadsheet.fromFile @"C:\Users\Mauso\OneDrive\CSB-Stuff\NFDI\Template-Skripts\1SPL01_plants_bearbeitungsCopy.xlsx" true
initErSheet ers.[0] ss

/// Adds a StyleSheet to a given SpreadsheetDocument
let addStyleSheet ss =
    let styleSheet = Stylesheet()
    styleSheet.HasChildren

/// Takes a WorkbookPart, a WorksheetName, and the index of a column and sets its width to best fit.
let setColWidthToBestFit wbp wsName =
    let wbp = Spreadsheet.getWorkbookPart ss
    let wsName = ers.[0]

    let wb = Workbook.get wbp
    let sheets = Sheet.Sheets.get wb
    let sheetIds = Sheet.Sheets.getSheets sheets |> Array.ofSeq
    let idOfMatchingSheetName = sheetIds |> Array.pick (fun t -> if t.Name.Value = wsName then Some t.Id.Value else None)
    let thisWs = Worksheet.WorksheetPart.getByID idOfMatchingSheetName wbp |> Worksheet.get
    let sd = Worksheet.getSheetData thisWs
    //let allCols = thisWs.GetFirstChild<Columns>()
    let allCols = Columns()
    let protoCols =
        Array.init 15 (
            fun _ -> 
                let protoCol = Column()
                protoCol.BestFit <- BooleanValue true
                protoCol
        )
    allCols.AddChild protoCols.[0] |> ignore
    for i = 1 to protoCols.Length - 1 do allCols.AppendChild protoCols.[i] |> ignore
    //thisWs.InsertBefore(allCols, sd)
    //thisWs.InsertAt(allCols, 0)
    thisWs.AppendChild allCols

Spreadsheet.close ss


let testSs = Spreadsheet.init "sheet1" @"C:\Users\Mauso\OneDrive\CSB-Stuff\testFiles\testExcelCreated4.xlsx"
let ws = Spreadsheet.tryGetWorksheetPartBySheetName "sheet1" testSs |> Option.get |> Worksheet.get
let sd = Spreadsheet.tryGetSheetBySheetName "sheet1" testSs |> Option.get
let allCols = Columns()
let protoCols =
    Array.init 2 (
        fun i -> 
            let protoCol = Column()
            let minmax = UInt32Value(uint i + 1u)
            protoCol.Min <- minmax
            protoCol.Max <- minmax
            protoCol.Width <- DoubleValue 7. // TO DO: Entweder alle Spaltenbreiten für jeden Calibri, Size 11-Char erlangen, in einem Dictionary hinterlegen und aus dem Text erzeugen lassen oder für alle Spalten in einem generischen ER-Sheet einmalig die Spaltenbreiten checken und hardcoden
            protoCol.CustomWidth <- BooleanValue true
            protoCol.BestFit <- BooleanValue true
            protoCol
    )
allCols.AddChild protoCols.[0] |> ignore
for i = 1 to protoCols.Length - 1 do allCols.AppendChild protoCols.[i] |> ignore
ws.InsertAt(allCols, 0)
let info = [|"Hello"; "World"|]
let row = Row.ofValues None 1u info
SheetData.appendRow row sd
Spreadsheet.close testSs

#r @"C:\Users\Mauso\.nuget\packages\system.drawing.common\5.0.2\runtimes\win\lib\netcoreapp3.0\System.Drawing.Common.dll"
open System.Drawing
#r @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Windows.Forms.dll"
open System.Windows.Forms

let font = new Font("Calibri", 11.0f, FontStyle.Regular)

// ------------------------------------------------------------------------------------------------
// newDoc Testwiese:
// ------------------------------------------------------------------------------------------------

let fullpath =
    let userProfile = System.Environment.SpecialFolder.UserProfile |> Environment.GetFolderPath
    Path.Combine(userProfile, @"OneDrive\CSB-Stuff\testFiles\testExcelCreated.xlsx")

let newDoc = // zuerst neues SpreadsheetDocument erstellen
    //SpreadsheetDocument.Create(fullpath, SpreadsheetDocumentType())
    SpreadsheetDocument.Create(fullpath, SpreadsheetDocumentType.Workbook)

newDoc.Close(); File.Delete fullpath // kill newDoc

let newWbp = Spreadsheet.initWorkbookPart newDoc // WorkbookPart zum SpreadsheetDocument hinzufügen
Workbook.init newWbp // Workbook dem WorkbookPart anhängen
let newWb = Workbook.get newWbp // hier wird das neue Workbook nur gebindet
Sheet.Sheets.init newWb // Sheets-Objekt zum Workbook hinzufügen
let newSheets = Sheet.Sheets.get newWb

let newWsp = WorkbookPart.initWorksheetPart newWbp // neues WorksheetPart zum WorkbookPart hinzufügen
Worksheet.init newWsp // neues Worksheet ans WorksheetPart angehängt
let newWs = Worksheet.get newWsp // neues Worksheet wird gebindet
let newSheetData = SheetData.empty () // neue SheetData

let newCells = 
    //let rows = 
    //    let cells = // das hier besser direkt schon im emptyErTable initiieren und nicht erst hier so hässlich umformen
    //        emptyErTable
    //        |> Array2D.mapi (
    //            fun i j s -> 
    //                let colRef = toExcelLetters (i + 1)
    //                let rowRef = (j + 1)
    //                let cellValue = Cell.CellValue.create s
    //                Cell.create CellValues.String ($"{colRef}{rowRef}") cellValue
    //                //Cell.setReference ($"{colRef}{rowRef}") newCell |> ignore
    //                //Cell.setValue cellValue newCell
    //        )
    //    let spans =
    //        let colL = Array2D.length2 emptyErTable
    //        Row.Spans.fromBoundaries 1u (uint colL)
    //    cells
    //    |> Array2D.toJaggedArray
    //    |> Array.mapi (
    //        fun i r ->
    //            Row.create (uint i + 1u) spans r
    //            |> SheetData.appendRow <| newSheetData
    //    )
    //rows

    let testCells =
        emptyErTable
        |> Array2D.mapi (
            fun i j s -> 
                let colRef = toExcelLetters (i + 1)
                let rowRef = (j + 1)
                let cellValue = Cell.CellValue.create s
                Cell.create CellValues.String ($"{colRef}{rowRef}") cellValue
        )
        |> fun arr2d -> arr2d.[0,0 ..]
    testCells |> Array.map (fun c -> c |> Cell.getCellValue |> fun x -> x.Text) |> ignore
    let testSpans =
        let colL = Array2D.length2 emptyErTable
        Row.Spans.fromBoundaries 1u (uint colL)
    testSpans.InnerText |> ignore
    let testRow = Row.create 1u testSpans testCells
    
    //SheetData.appendRow testRow newSheetData
    newSheetData.AppendChild(testRow)
    //()

    //|> Array.rev
    //|> Array.map (fun r -> SheetData.appendRow r newSheetData)



Worksheet.addSheetData newSheetData newWs // SheetData wird zum Worksheet hinzugefügt
let idOfNewWsp = newWbp.GetIdOfPart(newWsp) // ID des neuen WorksheetParts binden
let newSheet = Sheet.empty() // neues Sheet
Sheet.setID idOfNewWsp newSheet // die WorksheetPartID wird mit dem neuen Sheet verknüpft
Sheet.setSheetID 1u newSheet // wir geben dem neuen Sheet die Position 1 im Workbook
Sheet.setName "neues Sheet" newSheet // wir geben dem neuen Sheet einen Namen (, der auch in der Worksheet-Leiste in Excel angezeigt wird)
newSheets.AppendChild(newSheet) // das neue Sheet wird dem Sheets-Objekt hinzugefügt

//Sheet.add newDoc newSheet

newWs.Save()
newWb.Save() 
Sheet.countSheets newDoc

newDoc.Close()

SheetData.getCellAt 1u 1u newSheetData
//|> Cell.getCellValue
|> Cell.getValue None



let myRow = (newSheetData.ChildElements.Item 0) :?> Row
myRow.Elements<Cell>() |> Seq.map (fun c -> c.CellValue.Text) |> Array.ofSeq // passt


let newDoc2 = Spreadsheet.fromFile fullpath false






// noch härteres ToyExample

// C#
let mySpreadsheetDocument = SpreadsheetDocument.Create(@"C:\Users\Mauso\OneDrive\CSB-Stuff\testFiles\testExcelCreated2.xlsx", SpreadsheetDocumentType.Workbook)

// F#Spreadsheet
let thisSpreadsheetDocument = Spreadsheet.initEmpty @"C:\Users\Mauso\OneDrive\CSB-Stuff\testFiles\testExcelCreated3.xlsx"

// C#
let myWbp = mySpreadsheetDocument.AddWorkbookPart()
let myWb = Workbook()
myWbp.Workbook <- myWb

// F#Spreadsheet
let thisWbp = Spreadsheet.initWorkbookPart thisSpreadsheetDocument
let thisWb = Workbook.empty ()
Workbook.set thisWb thisWbp

// C#
let myWsp = myWbp.AddNewPart<WorksheetPart>()
let mySheetData = SheetData()
let myWs = Worksheet(mySheetData)

// F#Spreadsheet
let thisWsp = WorkbookPart.initWorksheetPart thisWbp
let thisSheetData = SheetData.empty ()
let thisWs = Worksheet.empty ()
Worksheet.setWorksheet thisWs thisWsp
Worksheet.addSheetData thisSheetData thisWs

// C#
let mySheets = Sheets()
mySpreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(mySheets)

// F#Spreadsheet
let thisSheets = Sheet.Sheets.empty ()
thisWb.Sheets <- thisSheets
Sheet.Sheets.get thisWb // hat (check)

// C#
let mySheet = Sheet()
mySheet.Id <- StringValue(mySpreadsheetDocument.WorkbookPart.GetIdOfPart(myWsp))
mySheet.SheetId <- UInt32Value(1u)
mySheet.Name <- StringValue("mySheet")
mySheets.Append(mySheet)

// F#Spreadsheet
let thisSheet = Sheet.empty ()
let idOfWsp = WorkbookPart.getWorksheetPartID thisWsp thisWbp
Sheet.setID idOfWsp thisSheet
Sheet.setSheetID 1u thisSheet
Sheet.setName "mySheet" thisSheet
Sheet.Sheets.addSheets [thisSheet] thisSheets // klappt nicht! s. u.

Sheet.Sheets.getFirstSheet thisSheets
thisSheets.FirstChild
thisSheets.Elements<Sheet>() |> Seq.head |> fun x -> x.Name
Sheet.countSheets thisSpreadsheetDocument

thisSheets.AddChild thisSheet // geht (anscheinend)

// C#
let myRow = Row()
myRow.RowIndex <- UInt32Value(1u)
mySheetData.Append myRow

// F#Spreadsheet
let thisRow = Row.empty ()
Row.setIndex 1u thisRow
SheetData.appendRow thisRow thisSheetData

thisSheetData.Elements<Row>() |> Seq.head |> fun x -> x.RowIndex // passt

// C#
let mutable (refCell : Cell) = null
for i in myRow.Elements<Cell>() do
    if String.Compare(i.CellReference.Value, "A1", true) > 0 then
        refCell <- i
let newCell = Cell()
newCell.CellReference <- StringValue("A1")
myRow.InsertBefore(newCell, refCell)

// F#Spreadsheet
let mutable (refCell : Cell) = Cell()
refCell.CellReference <- StringValue("A1")
let thisCell = Cell.empty ()
Cell.setReference "A1" thisCell
Row.insertCellBefore thisCell refCell thisRow // geht nicht
let thisSpans = Row.Spans.fromBoundaries 1u 1u
//Row.create 1u thisSpans [thisCell]

//thisRow.Remove()
//let thisRow = Row.create 1u thisSpans [thisCell]

Row.appendCell thisCell thisRow

// C#
newCell.CellValue <- CellValue("100")
newCell.DataType <- EnumValue<CellValues>(CellValues.Number)

// F#Spreadsheet
Cell.setValue (CellValue "100") thisCell
Cell.setType CellValues.Number thisCell

let newCell2 = Cell()
Cell.setReference "A2" newCell2
Cell.setValue (CellValue ("69")) thisCell

// C#
mySpreadsheetDocument.Close()

// F#Spreadsheet
Spreadsheet.close thisSpreadsheetDocument


// Pipeline for ..Created3.xlsx

let thisSpreadsheetDocument = Spreadsheet.initEmpty @"C:\Users\Mauso\OneDrive\CSB-Stuff\testFiles\testExcelCreated3.xlsx"

let thisWbp = Spreadsheet.initWorkbookPart thisSpreadsheetDocument
let thisWb = Workbook.empty ()
Workbook.set thisWb thisWbp

let thisWsp = WorkbookPart.initWorksheetPart thisWbp
let thisSheetData = SheetData.empty ()
let thisWs = Worksheet.empty ()
Worksheet.setWorksheet thisWs thisWsp
Worksheet.addSheetData thisSheetData thisWs

let thisSheets = Sheet.Sheets.empty ()
thisWb.Sheets <- thisSheets

Sheet.Sheets.getSheets thisSheets |> Array.ofSeq |> Seq.map (fun s -> s.Name, s.Id, s.SheetId)

let thisSheet = Sheet.empty ()
let idOfWsp = WorkbookPart.getWorksheetPartID thisWsp thisWbp
Sheet.setID idOfWsp thisSheet
Sheet.setSheetID 1u thisSheet
Sheet.setName "mySheet" thisSheet
thisSheets.AddChild thisSheet 

let thisRow = Row.empty ()
Row.setIndex 1u thisRow
SheetData.appendRow thisRow thisSheetData

let thisCell = Cell.empty ()
Cell.setReference "A1" thisCell
Cell.setValue (CellValue "100") thisCell
Row.appendCell thisCell thisRow

let thisCell2 = Cell()
Cell.setReference "B1" thisCell2
Cell.setValue (CellValue ("69")) thisCell2
Row.appendCell thisCell2 thisRow

let thisCell3 = Cell.empty ()
Cell.setReference "Q1" thisCell3
Cell.setValue (CellValue ("1337")) thisCell3
Row.appendCell thisCell3 thisRow

let thisRow2 = Row.empty ()
Row.setIndex 3u thisRow2
SheetData.appendRow thisRow2 thisSheetData

let thisCell4 = Cell.empty ()
Cell.setReference "C3" thisCell4
Cell.setValue (CellValue ("9001")) thisCell4
Row.appendCell thisCell4 thisRow2

let thisCell5 = Cell.empty ()
Cell.setValue (CellValue ("23")) thisCell5

let thisCell6 = thisCell5.Clone() :?> Cell

Row.appendCell thisCell5 thisRow
Row.appendCell thisCell6 thisRow2

let thisWsp2 = WorkbookPart.initWorksheetPart thisWbp
let thisSheetData2 = SheetData.empty ()
let thisWs2 = Worksheet.empty ()
Worksheet.setWorksheet thisWs2 thisWsp2
Worksheet.addSheetData thisSheetData2 thisWs2

let thisSheet2 = Sheet.empty ()
let idOfWsp2 = WorkbookPart.getWorksheetPartID thisWsp2 thisWbp
Sheet.setID idOfWsp2 thisSheet2
Sheet.setSheetID 2u thisSheet2
Sheet.setName "mySheet2" thisSheet2
thisSheets.AppendChild thisSheet2

let thisRow3 = Row.empty ()
Row.setIndex 1u thisRow3
SheetData.appendRow thisRow3 thisSheetData2

let thisCell7 = Cell.empty ()
Cell.setReference "A1" thisCell7
Cell.setValue (CellValue "7") thisCell7
Row.appendCell thisCell7 thisRow3

Spreadsheet.close thisSpreadsheetDocument




// Pipeline for ..bearbeitungsCopy.xlsx

Spreadsheet.saveAs @"C:\Users\Mauso\OneDrive\CSB-Stuff\NFDI\Template-Skripts\1SPL01_plants_bearbeitungsCopy.xlsx" testTemplate

let thisSpreadsheetDocument = Spreadsheet.fromFile @"C:\Users\Mauso\OneDrive\CSB-Stuff\NFDI\Template-Skripts\1SPL01_plants_bearbeitungsCopy.xlsx" true

let thisWbp = Spreadsheet.getWorkbookPart thisSpreadsheetDocument
let thisWb = Workbook.get thisWbp

let thisWsp = WorkbookPart.initWorksheetPart thisWbp
let thisSheetData = SheetData.empty ()
let thisWs = Worksheet.empty ()
Worksheet.setWorksheet thisWs thisWsp
Worksheet.addSheetData thisSheetData thisWs

let thisSheets = Sheet.Sheets.get thisWb

let thisSheet = Sheet.empty ()
let idOfWsp = WorkbookPart.getWorksheetPartID thisWsp thisWbp
Sheet.setID idOfWsp thisSheet
Sheet.setSheetID 1u thisSheet
Sheet.setName "mySheet" thisSheet
thisSheets.AppendChild thisSheet 

let thisRow = Row.empty ()
Row.setIndex 1u thisRow
SheetData.appendRow thisRow thisSheetData

let thisCell = Cell.empty ()
Cell.setReference "A1" thisCell
Cell.setValue (CellValue "100") thisCell
Row.appendCell thisCell thisRow

let thisCell2 = Cell()
Cell.setReference "B1" thisCell2
Cell.setValue (CellValue ("69")) thisCell2
Row.appendCell thisCell2 thisRow

let thisCell3 = Cell.empty ()
Cell.setReference "Q1" thisCell3
Cell.setValue (CellValue ("1337")) thisCell3
Row.appendCell thisCell3 thisRow

let thisRow2 = Row.empty ()
Row.setIndex 3u thisRow2
SheetData.appendRow thisRow2 thisSheetData

let thisCell4 = Cell.empty ()
Cell.setReference "C3" thisCell4
Cell.setValue (CellValue ("9001")) thisCell4
Row.appendCell thisCell4 thisRow2

let thisCell5 = Cell.empty ()
Cell.setValue (CellValue ("23")) thisCell5

let thisCell6 = thisCell5.Clone() :?> Cell

Row.appendCell thisCell5 thisRow
Row.appendCell thisCell6 thisRow2

let thisWsp2 = WorkbookPart.initWorksheetPart thisWbp
let thisSheetData2 = SheetData.empty ()
let thisWs2 = Worksheet.empty ()
Worksheet.setWorksheet thisWs2 thisWsp2
Worksheet.addSheetData thisSheetData2 thisWs2

let thisSheet2 = Sheet.empty ()
let idOfWsp2 = WorkbookPart.getWorksheetPartID thisWsp2 thisWbp
Sheet.setID idOfWsp2 thisSheet2
Sheet.setSheetID 2u thisSheet2
Sheet.setName "mySheet2" thisSheet2
thisSheets.AppendChild thisSheet2

let thisRow3 = Row.empty ()
Row.setIndex 1u thisRow3
SheetData.appendRow thisRow3 thisSheetData2

let thisCell7 = Cell.empty ()
Cell.setReference "A1" thisCell7
Cell.setValue (CellValue "7") thisCell7
Row.appendCell thisCell7 thisRow3

Spreadsheet.close thisSpreadsheetDocument


JaggedArray.init 3 7 (fun i j -> i * 2, j * 3)


// TO DO: Vesuchen, irgendwie die SheetID aus dem WorksheetPart zu bekommen, sodass am Ende SheetName und Worksheet(Part) gematcht werden können.
// Ist wichtig, um später Sheets herstellen und mit den entsprechenden hergestellten Worksheet(Part)s verknüpfen zu können.
let matchSheetIDWithWorksheet =
    wspSwateTable
    |> Worksheet.WorksheetPart.

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
let fullErTable =