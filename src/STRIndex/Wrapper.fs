namespace STRIndex

open ARCtrl
open ARCtrl.Json


module Wrapper =
    
    let templateToJson (template: Template) : string =
        let json = Template.toJsonString 0 (template)
        json

    let templateFromJson (json: string) : Template =
        let template = Template.fromJsonString(json)
        template

    open ARCtrl.Spreadsheet
    open ARCtrl.Spreadsheet.ArcTable

    let tableToRowCellStringList (table: ArcTable) : string list list =
        table.Columns
        |> List.ofArray
        |> List.sortBy classifyColumnOrder
        |> List.collect CompositeColumn.toStringCellColumns
        |> List.transpose

