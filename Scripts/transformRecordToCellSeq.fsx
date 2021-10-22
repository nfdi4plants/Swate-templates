#r "nuget: ISADotNet.Xlsx, 0.2.6"
#r "nuget: FSharpSpreadsheetML, 0.0.4"

open FSharpSpreadsheetML
open DocumentFormat.OpenXml.Spreadsheet

let file = ISADotNet.XLSX.AssayFile.AssayFile.fromFile @"C:\Users\olive\OneDrive\CSB-Stuff\NFDI\Template-Skripts\1SPL01_plants - Copy.xlsx"
let file2 = ISADotNet.XLSX.AssayFile.AssayFile.fromFile @"C:\Users\olive\OneDrive\CSB-Stuff\NFDI\testARC\testAssayFile.xlsx"

/// Takes an instance of a generic RecordType and returns a sequence of Cells according to the FieldNames of the Record.
let recordToCells record =

    // get fields of the Record
    let sysType = record.GetType()
    let fields = FSharp.Reflection.FSharpType.GetRecordFields(sysType)

    // transform dataType, cellReference and the field into a cell
    let toCell (valType : CellValues option) (ref : string option) (value : string) = 
        let cell = Cell.empty ()
        if ref.IsSome       then Cell.setReference  ref.Value       cell |> ignore
        if valType.IsSome   then Cell.setType       valType.Value   cell |> ignore
        let (cv : CellValue) = 
            if valType.IsSome then
                match valType.Value with
                | CellValues.Boolean        -> CellValue(System.Convert.ToBoolean value)
                | CellValues.String
                | CellValues.SharedString
                | CellValues.InlineString   -> CellValue(value)
                | CellValues.Number         -> CellValue(float value)
                | CellValues.Error          -> CellValue("error")
                | CellValues.Date           -> CellValue(System.Convert.ToDateTime value)
                | _                         -> CellValue(value)
            else CellValue(value)
        Cell.setValue cv cell

    // create Option seq with Nones if dataTypes and/or cellReferences is None
    let createOptionSeq (optionParameter : 'a option) =
        match optionParameter.IsSome with
        | true  -> optionParameter.Value
        | false -> Seq.init fields.Length (fun _ -> None)
    let dts = createOptionSeq dataTypes
    let crs = createOptionSeq cellReferences

    // execute
    (fields, dts, crs)
    |||> Seq.map3 (fun field dt cellRef -> toCell dt cellRef field.Name)

/// Takes an instance of a generic RecordType and returns a Row of the according FieldNames of the Record.
let recordToHeader index (* <- in die PipelineFunktion *) record =

    // get fields of the Record
    let sysType = record.GetType()
    let fields = FSharp.Reflection.FSharpType.GetRecordFields(sysType)
    
    // create Row
    fields
    |> Seq.map (fun field -> field.Name)
    |> Row.ofValues None index

type MyRecord = {
    eins    : int
    zwei    : float
    drei    : string
}

let myRecordInstance = {eins = 1; zwei = 2.; drei = "3"}

let cellSeq = transformRecordToCellSeq None None myRecordInstance |> Seq.toArray

let row = transformRecordToRow 1u myRecordInstance

cellSeq.[2].CellValue.Text
row.Elements<Cell>() |> Seq.iter (fun x -> printfn "%s" x.CellValue.Text)

let assay = 
    let ass = file2 |>  fun (_,_,_,d) -> d
    ass.ProcessSequence