#r "nuget: ISADotNet.Xlsx, 0.2.6"
#r "nuget: FSharpSpreadsheetML, 0.0.4"

open FSharpSpreadsheetML
open DocumentFormat.OpenXml.Spreadsheet
open FSharp.Reflection
open System
open System.Reflection

let file = ISADotNet.XLSX.AssayFile.AssayFile.fromFile @"C:\Users\olive\OneDrive\CSB-Stuff\NFDI\Template-Skripts\1SPL01_plants - Copy.xlsx"
let file2 = ISADotNet.XLSX.AssayFile.AssayFile.fromFile @"C:\Users\olive\OneDrive\CSB-Stuff\NFDI\testARC\testAssayFile.xlsx"

type ValType = {
    CellValueType   : CellValues
    CellValue       : CellValue
}

let createValType cvt cv = {CellValueType = cvt; CellValue = cv}

let getOptionSome (opt : option<_>) = if opt.IsSome then box opt.Value else box ""




// TO DO: functionality for Attributes that give an order (e.g. field1 should be transformed to cell1
/// Takes an instance of a generic RecordType and returns a sequence of Cells according to the FieldNames of the Record.
let recordToCells record =

    // get fields of the Record
    let sysType = record.GetType()
    let fields = FSharpType.GetRecordFields(sysType)

    // match Record field type to get CellValues case.
    let getValType (field : PropertyInfo) =
        let typedef = field.PropertyType.GetGenericTypeDefinition()
        match typedef with
        | x when
            x = typedefof<unit>         ||
            x = typedefof<option<unit>> ||
            x = typedefof<string>       ||
            x = typedefof<option<string>>       ||
            x = typedefof<char>
            -> CellValues.String
        | x when 
            x = typedefof<int>      ||
            x = typedefof<uint>     ||
            x = typedefof<float>    ||
            x = typedefof<decimal>  ||
            x = typedefof<byte>
            -> CellValues.Number
        | x when x = typedefof<bool>
            -> CellValues.Boolean
        | x when x = typedefof<DateTime>
            -> CellValues.Date
        | _ -> CellValues.Error // alternativ: failwith "ERROR: Cannot match input field of Record."

    // transform field properties into a cell
    let toCell (field : PropertyInfo) = 
        let cell = Cell.empty ()
        let valType = getValType field
        Cell.setType valType cell |> ignore
        let value = FSharpValue.GetRecordField(record, field)
        let matchOption v = 
            if isNull v then true
            elif v 
        let (cv : CellValue) = 
            match valType with
            | CellValues.Boolean        -> CellValue(unbox<bool>        value)
            | CellValues.String         -> CellValue(unbox<string>      value)
            | CellValues.Number         -> CellValue(unbox<float>       value)
            | CellValues.Date           -> CellValue(unbox<DateTime>    value)
            | CellValues.Error          -> CellValue("<unknownDataType>") // alternativ: failwith oben und diesen Case nicht matchen
            | _                         -> failwith "ERROR: Impossible valType given."
        Cell.setValue cv cell

    // execute
    Seq.map toCell fields

// Index in die Pipeline-Funktion, pipen into Row.updateRowIndex (macht, was der Name sagt)
/// Takes an instance of a generic RecordType and returns a header-row of the according FieldNames of the Record.
let recordToHeader record =

    // get fields of the Record
    let sysType = record.GetType()
    let fields = FSharpType.GetRecordFields(sysType)
    
    // create Row
    fields
    |> Seq.map (fun field -> field.Name)
    |> Row.ofValues None 1u

type MyRecord = {
    eins    : int
    zwei    : float
    drei    : string
    vier    : bool
    fünf    : uint
    sechs   : byte
    sieben  : char
    acht    : decimal
    neun    : unit
    zehn    : double
    elf     : System.DateTime
    zwölf   : int option
    drei10  : string option
    vier10  : float option
}

let myRecordInstance = {eins = 1; zwei = 2.; drei = "3"; vier = true; fünf = 5u; sechs = 6uy; sieben = '7'; acht = 8m; neun = (); zehn = 10.; elf = System.DateTime.Now; zwölf = Some 12; drei10 = Some "13"; vier10 = None}

let sysType = myRecordInstance.GetType()
let fields = FSharp.Reflection.FSharpType.GetRecordFields(sysType)

let getValType record (field : PropertyInfo) = 

    // option type matching
    let matchOptionType value (field : PropertyInfo) =
        let baseTypeName = 
            field
                .PropertyType
                .GetProperty("Value")
                .PropertyType
                .Name
        match baseTypeName with
        | "Int32" -> 
            let isSome = unbox<int option> value |> Option.isSome
            if isSome then CellValue(unbox<int option> value |> Option.get |> float) else CellValue("")
            |> createValType CellValues.Number
        | "UInt32" -> 
            let isSome = unbox<uint option> value |> Option.isSome
            if isSome then CellValue(unbox<uint option> value |> Option.get |> float) else CellValue("")
            |> createValType CellValues.Number
        | "Double" -> 
            let isSome = unbox<float option> value |> Option.isSome
            if isSome then CellValue(unbox<float option> value |> Option.get) else CellValue("")
            |> createValType CellValues.Number
        | "Byte" -> 
            let isSome = unbox<byte option> value |> Option.isSome
            if isSome then CellValue(unbox<byte option> value |> Option.get |> float) else CellValue("")
            |> createValType CellValues.Number
        | "Decimal" -> 
            let isSome = unbox<decimal option> value |> Option.isSome
            if isSome then CellValue(unbox<decimal option> value |> Option.get |> float) else CellValue("")
            |> createValType CellValues.Number
        | "String" -> 
            let isSome = unbox<string option> value |> Option.isSome
            if isSome then CellValue(unbox<string option> value |> Option.get) else CellValue("")
            |> createValType CellValues.String
        | "Char" -> 
            let isSome = unbox<char option> value |> Option.isSome
            if isSome then CellValue(unbox<char option> value |> Option.get |> string) else CellValue("")
            |> createValType CellValues.String
        | "Boolean" -> 
            let isSome = unbox<bool option> value |> Option.isSome
            if isSome then CellValue(unbox<bool option> value |> Option.get) else CellValue("")
            |> createValType CellValues.Boolean
        | "DateTime" -> 
            let isSome = unbox<DateTime option> value |> Option.isSome
            if isSome then CellValue(unbox<DateTime option> value |> Option.get) else CellValue("")
            |> createValType CellValues.Date
        | "Unit"    -> createValType CellValues.String  (CellValue(""))
        | _         -> createValType CellValues.Error   (CellValue("<unknownDataType>"))

    // type matching
    let typeName = field.PropertyType.Name
    let value = FSharpValue.GetRecordField(record, field)
    match typeName with
    | "Int32"           -> createValType CellValues.Number  (CellValue(unbox<int> value |> float))
    | "UInt32"          -> createValType CellValues.Number  (CellValue(unbox<uint> value |> float))
    | "Double"          -> createValType CellValues.Number  (CellValue(unbox<float> value))
    | "Byte"            -> createValType CellValues.Number  (CellValue(unbox<byte> value |> float))
    | "Decimal"         -> createValType CellValues.Number  (CellValue(unbox<decimal> value |> float))
    | "String"          -> createValType CellValues.String  (CellValue(unbox<string> value))
    | "Char"            -> createValType CellValues.String  (CellValue(unbox<char> value |> string))
    | "Unit"            -> createValType CellValues.String  (CellValue(""))
    | "Boolean"         -> createValType CellValues.Boolean (CellValue(unbox<bool> value))
    | "DateTime"        -> createValType CellValues.Date    (CellValue(unbox<DateTime> value))
    | "FSharpOption`1"  -> matchOptionType value field
    | _                 -> createValType CellValues.Error   (CellValue("<unknownDataType>"))

for i in fields do printfn "%A" i.Name; getValType myRecordInstance i |> fun s -> s.CellValue.Text |> printfn "%A"

let cellSeq = transformRecordToCellSeq None None myRecordInstance |> Seq.toArray

let row = transformRecordToRow 1u myRecordInstance

cellSeq.[2].CellValue.Text
row.Elements<Cell>() |> Seq.iter (fun x -> printfn "%s" x.CellValue.Text)

let assay = 
    let ass = file2 |>  fun (_,_,_,d) -> d
    ass.ProcessSequence