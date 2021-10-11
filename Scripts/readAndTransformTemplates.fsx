#r "nuget: FSharpSpreadsheetML"
#r "nuget: FSharpAux"

open FSharpSpreadsheetML
open DocumentFormat.OpenXml.Spreadsheet
open FSharpAux
open System

type CVEntry = {
    Ontology        : string
    TAN             : string
    TermSourceRef   : string
}

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

module String =
    /// Returns the first char of a string.
    let first (str : string) = str.Chars 0
    /// Returns the last char of a string.
    let last (str : string) = str.Chars (str.Length - 1)
    /// Splits an input string at a given delimiter (substring).
    let splitS (delimiter : string) (str : string) = str.Split ([|delimiter|],System.StringSplitOptions.None)
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
        |> Array.takeWhile System.Char.IsLetter
        |> System.String
        |> toNumber,
        chArr
        |> Array.skipWhile System.Char.IsLetter
        |> System.String
        |> int
    cRow = row && cCol >= colL && cCol <= colR

let templates =
    let dirs = System.IO.Directory.GetDirectories "../templates/"
    let files = dirs |> Array.collect (fun d -> System.IO.Directory.GetFiles(d,"*.xlsx")) 
    files |> Array.map (fun f -> Spreadsheet.fromFile f false)

templates |> Array.iter (fun d -> d.Close())

let testTemplate = templates.[0]

let sst = Spreadsheet.getSharedStringTable testTemplate

let wsp =
    Spreadsheet.getWorkbookPart testTemplate
    |> WorkbookPart.getWorkSheetParts
    |> Seq.head

let swateTable = (Table.tryGetByNameBy ((=) "annotationTable") wsp).Value

let sd = Worksheet.get wsp |> Worksheet.getSheetData

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

