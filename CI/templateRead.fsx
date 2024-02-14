#r "nuget: ARCtrl.NET, 1.0.4"

open System.IO
open FsSpreadsheet
open FsSpreadsheet.ExcelIO
open ARCtrl.Template
open ARCtrl.NET

let source = __SOURCE_DIRECTORY__ 
let inputPath = Path.Combine(source, "..", "templates")
let options = EnumerationOptions()
options.RecurseSubdirectories <- true
let files = Directory.GetFiles(inputPath,"*.xlsx",enumerationOptions=options)

let templates =
  files 
  |> Array.map (FsWorkbook.fromXlsxFile >> Spreadsheet.Template.fromFsWorkbook)